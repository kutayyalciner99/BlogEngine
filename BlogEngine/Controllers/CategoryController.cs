using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly BlogeEngineDbContext context; //bu değişkene atamamızın sebebi class içinde herhangi bi yerde kullanabilmek için yazılır. Read-only atama yaptırmıyor, get set oluşturmuyor(Sadece constructor da atama yapılıyor read only olduğu için)
        public CategoryController(BlogeEngineDbContext context)//context instance'ı startup ta tanıtılmış olmalı.singletone,tek instance ile birden çok işlem için tek context ile yapmak(Get,post,
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get() //async ekleniyorsa, scope içinde "awaitable" bir method kullanılmalıdır.
        {
           
            return await context.Categories.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            return await context.Categories.FindAsync(id);
        }
        [HttpPost]
        public async Task<ActionResult> Post(Category category)
        {
            context.Entry(category).State = EntityState.Added;
            try
            {
                await context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Kategori ismi zaten var...");
            }
            return Ok();
        }
        [HttpPut]
        public async Task<ActionResult> Put(Category category)
        {
            context.Entry(category).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Kategori ismi zaten var...");
            }
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var category = context.Categories.Find(id);
            if (category == null)
            {
                return BadRequest("Silinmek istenen kayıt bulunamadı.");
            }
            context.Entry(category).State = EntityState.Deleted;
            try
            {
                await context.SaveChangesAsync();

            }
            catch (DbUpdateException)
            {
                return BadRequest("Bu kategoriye bağlı gönderiler olduğu için bu kategori silinemiyor");
            }
            return Ok();
        }

    }
}