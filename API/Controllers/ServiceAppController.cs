using API.Dtos;
using AutoMapper;
using Core.Entity;
using Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceAppController : ControllerBase
    {
        private readonly IServiceAppRepository _serviceAppRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public ServiceAppController(
            IServiceAppRepository serviceAppRepository,
            ICategoryRepository categoryRepository,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _serviceAppRepository = serviceAppRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        // جلب قائمة الخدمات مع الفلترة والترتيب والترقيم
        [HttpGet]
        public async Task<ActionResult<List<ServiceAppDto>>> GetServiceAppListAsync(
            [FromQuery] string? query,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            var services = await _serviceAppRepository.GetServiceApplist(query, sortBy, sortDirection, pageNumber, pageSize);
            var response = _mapper.Map<List<ServiceAppDto>>(services);
            return Ok(response);
        }

        // جلب إجمالي عدد الخدمات
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> GetCategoriesTotal()
        {
            var count = await _serviceAppRepository.GetCount();
            return Ok(count);
        }

        // جلب تفاصيل خدمة بواسطة المعرف الرقمي
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetServiceAppById([FromRoute] int id)
        {
            var existingServiceApp = await _serviceAppRepository.GetServiceAppById(id);

            if (existingServiceApp is null)
            {
                return NotFound();
            }

            var response = _mapper.Map<ServiceAppDto>(existingServiceApp);
            return Ok(response);
        }

        // جلب تفاصيل خدمة بواسطة الرابط الدائم (UrlHandle)
        [HttpGet("{urlHandle}")]
        public async Task<IActionResult> GetServiceAppByUrlHandle([FromRoute] string urlHandle)
        {
            var existingServiceApp = await _serviceAppRepository.GetServiceAppByUrlHandleAsync(urlHandle);

            if (existingServiceApp is null)
            {
                return NotFound();
            }

            var response = _mapper.Map<ServiceAppDto>(existingServiceApp);
            return Ok(response);
        }

        // إضافة خدمة جديدة (محمي بتسجيل الدخول)
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ServiceAppDto>> AddListServiceAppAsync([FromBody] ServiceAppDtoRequest request)
        {
            var category = await _categoryRepository.GetById(request.CategoryId);
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null || category == null)
            {
                return NotFound("المستخدم أو التصنيف غير موجود.");
            }

            // توليد الرابط الدائم (Slug) مع دعم الأسماء العربية والإنجليزية والاحتياط
            string cleanName = Regex.Replace(request.Name.Trim().ToLower(), @"\s+", "-");
            cleanName = Regex.Replace(cleanName, @"[^a-z0-9\-\u0600-\u06FF]", "");
            cleanName = cleanName.Trim('-');

            string baseUrlHandle = string.IsNullOrWhiteSpace(cleanName) ? "service" : cleanName;
            string uniquePart = Guid.NewGuid().ToString("N").Substring(0, 8);
            string urlHandle = $"{baseUrlHandle}-{uniquePart}";

            var serviceApp = _mapper.Map<ServiceApp>(request);
            serviceApp.UrlHandle = urlHandle;

            await _serviceAppRepository.CreateAsync(serviceApp);

            var response = _mapper.Map<ServiceAppDto>(serviceApp);
            return CreatedAtAction(nameof(GetServiceAppById), new { id = response.Id }, response);
        }

        // تعديل خدمة قائمة (محمي بتسجيل الدخول)
        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> EditServiceApp([FromRoute] int id, [FromBody] UpdateServiceAppRequestDto request)
        {
            var existing = await _serviceAppRepository.GetServiceAppById(id);
            if (existing == null)
            {
                return NotFound();
            }

            var serviceApp = _mapper.Map<ServiceApp>(request);
            serviceApp.Id = id;
            serviceApp.UrlHandle = existing.UrlHandle;

            serviceApp = await _serviceAppRepository.UpdateServiceAppAsync(serviceApp);

            if (serviceApp == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<ServiceAppDto>(serviceApp);
            return Ok(response);
        }

        // حذف خدمة (محمي بتسجيل الدخول)
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var existingServiceApp = await _serviceAppRepository.GetServiceAppById(id);

            if (existingServiceApp is null)
            {
                return NotFound($"الخدمة برقم {id} غير موجودة.");
            }

            await _serviceAppRepository.DeleteServiceAppAsync(id);

            var response = _mapper.Map<ServiceAppDto>(existingServiceApp);
            return Ok(response);
        }
    }
}