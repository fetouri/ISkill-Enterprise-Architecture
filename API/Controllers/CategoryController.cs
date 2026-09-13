using API.Dtos;
using AutoMapper;
using Core.Entity;
using Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetListAsync()
        {
            var categories = await _categoryRepository.GetCategorylist();

            var response = _mapper.Map<List<CategoryDto>>(categories);

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            var existingCategory = await _categoryRepository.GetById(id);

            if (existingCategory is null)
            {
                return NotFound();
            }

            var response = _mapper.Map<CategoryDto>(existingCategory);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<ActionResult<CategoryDto>> AddListAsync([FromBody] CategoryDtoRequest request)
        {
            var category = _mapper.Map<Category>(request);

            await _categoryRepository.CreateAsync(category);

            var response = _mapper.Map<CategoryDto>(category);
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditCategory([FromRoute] int id, [FromBody] UpdateCategoryRequestDto request)
        {
            // Convert DTO to Domain Model and assign route ID
            var category = _mapper.Map<Category>(request);
            category.Id = id;

            var updatedCategory = await _categoryRepository.UpdateAsync(category);

            if (updatedCategory == null)
            {
                return NotFound();
            }

            // Convert Domain model to DTO
            var response = _mapper.Map<CategoryDto>(updatedCategory);

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var existingCategory = await _categoryRepository.GetById(id);

            if (existingCategory is null)
            {
                return NotFound();
            }

            await _categoryRepository.DeletAsync(id);

            var response = _mapper.Map<CategoryDto>(existingCategory);

            return Ok(response);
        }
    }
}