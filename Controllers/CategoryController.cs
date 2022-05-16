using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Shop.Models;
using Shop.Data;
using Microsoft.AspNetCore.Authorization;

// http://localhost:5000/

[Route("v1/categories")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] //desabilita o cache para o método, para casos em que a linha "services.AddResponseCaching();" das configurações está ativa
    public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
    {
        var category = await context.Categories.AsNoTracking().ToListAsync();
        return Ok(category);
    }
    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(int id, [FromServices]DataContext context)
    {
        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if(category == null)
            return NotFound(new { message = "Não foi possível encontrar essa categoria."});
        try{
            return Ok(category);
        }
        catch(Exception)
        {
            return BadRequest(new { message = "Erro ao retornar categoria."});
        }
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Post([FromBody]Category model,
    [FromServices]DataContext context
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
     
        try{
            context.Categories.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }catch
        {
            return BadRequest(new { message = "Não foi possível criar a categoria"});
        }
        
    }
    
    [HttpPut]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Put([FromBody]Category model, [FromServices]DataContext context)
    {
        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }
        try{
            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch(DbUpdateConcurrencyException){
            return BadRequest(new { message = "Erro ao atualizar categoria, essa categoria já está sendo atualizada."});
        }
        catch(Exception){
            return BadRequest(new { message = "Erro ao atualizar categoria."});
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Delete(int id, [FromServices]DataContext context)
    {
        var categoria = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if(categoria == null){
            return NotFound(new { message = "Não foi possível encontrar a categoria."});
        }
        try{
            context.Categories.Remove(categoria);
            await context.SaveChangesAsync();
            return Ok(new { message = "Categoria removida com sucesso."});
        }
        catch(DbUpdateConcurrencyException){
            return BadRequest(new { message = "Erro ao executar, essa categoria já está sendo removida."});
        }
        catch(Exception){
            return BadRequest(new { message = "Erro ao remover categoria."});
        }
    }
}