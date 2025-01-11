using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pokedex.Models;
using pokedex.Services;

namespace pokedex.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Pokemon>>> Get()
        {
            try
            {
                var pokemons = await _pokemonService.GetPokemonsAsync();
                return Ok(pokemons);
            }
            catch (ApplicationException e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pokemon>> GetPokemon(string id)
        {
            try
            {
                var pokemon = await _pokemonService.GetPokemonByIdAsync(id);
                return Ok(pokemon);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound($"Error: {e.Message}");
            }
            catch (ApplicationException e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpGet("search/{name}")]
        public async Task<ActionResult<Pokemon>> GetPokemonByName(string name)
        {
            try
            {
                var pokemon = await _pokemonService.GetPokemonByNameAsync(name);
                return Ok(pokemon);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound($"Error: {e.Message}");
            }
            catch (ApplicationException e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Pokemon>> AddPokemon([FromBody] Pokemon newPokemon)
        {
            try
            {
                var addedPokemon = await _pokemonService.AddPokemonAsync(newPokemon);
                return CreatedAtAction(nameof(GetPokemon), new { id = addedPokemon.Id }, addedPokemon);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest($"Error: {e.Message}");
            }
            catch (ApplicationException e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Pokemon>> UpdatePokemon(string id, [FromBody] Pokemon updatedPokemon)
        {
            try
            {
                var updated = await _pokemonService.UpdatePokemonAsync(id, updatedPokemon);
                return Ok(updated);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound($"Error: {e.Message}");
            }
            catch (ApplicationException e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePokemon(string id)
        {
            try
            {
                var success = await _pokemonService.DeletePokemonAsync(id);
                if (success)
                {
                    return Ok($"Deleted the Pokemon with id {id}");
                }
                return NotFound($"Error: Pokemon with Id {id} not found.");
            }
            catch (KeyNotFoundException e)
            {
                return NotFound($"Error: {e.Message}");
            }
            catch (ApplicationException e)
            {
                return StatusCode(500, $"Error: {e.Message}");
            }
        }
    }
}
