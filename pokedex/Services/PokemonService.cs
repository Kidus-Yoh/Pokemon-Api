using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using pokedex.Models;
using Microsoft.Extensions.Configuration;

namespace pokedex.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly IMongoCollection<Pokemon> _pokemonCollection;

        public PokemonService(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("MongoDB:ConnectionString").Value;
            var databaseName = configuration.GetSection("MongoDB:DatabaseName").Value;
            var collectionName = configuration.GetSection("MongoDB:Collection").Value;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _pokemonCollection = database.GetCollection<Pokemon>(collectionName);
        }

        public async Task<List<Pokemon>> GetPokemonsAsync()
        {
            try
            {
                return await _pokemonCollection.Find(pokemon => true).ToListAsync();
            }
            catch
            {
                throw new ApplicationException("An error occurred while fetching pokemons.");
            }
        }

        public async Task<Pokemon> GetPokemonByIdAsync(string id)
        {
            try
            {
                var pokemon = await _pokemonCollection.Find(pokemon => pokemon.Id == id).FirstOrDefaultAsync();
                if (pokemon == null)
                {
                    throw new KeyNotFoundException($"Pokemon with Id {id} not found.");
                }
                return pokemon;
            }
            catch
            {
                throw new ApplicationException("An error occurred while fetching the pokemon.");
            }
        }

        public async Task<Pokemon> GetPokemonByNameAsync(string name)
        {
            try
            {
                var pokemon = await _pokemonCollection.Find(pokemon => pokemon.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
                if (pokemon == null)
                {
                    throw new KeyNotFoundException($"Pokemon with name {name} not found.");
                }
                return pokemon;
            }
            catch
            {
                throw new ApplicationException("An error occurred while fetching the pokemon.");
            }
        }

        public async Task<Pokemon> AddPokemonAsync(Pokemon newPokemon)
        {
            try
            {
                var existingPokemon = await _pokemonCollection.Find(pokemon => pokemon.Name.Equals(newPokemon.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
                if (existingPokemon != null)
                {
                    throw new InvalidOperationException($"Pokemon with name {newPokemon.Name} already exists.");
                }

                newPokemon.Id = Guid.NewGuid().ToString();
                await _pokemonCollection.InsertOneAsync(newPokemon);
                return newPokemon;
            }
            catch
            {
                throw new ApplicationException("An error occurred while adding the pokemon.");
            }
        }

        public async Task<Pokemon> UpdatePokemonAsync(string id, Pokemon updatedPokemon)
        {
            try
            {
                var result = await _pokemonCollection.ReplaceOneAsync(pokemon => pokemon.Id == id, updatedPokemon);
                if (result.MatchedCount == 0)
                {
                    throw new KeyNotFoundException($"Pokemon with Id {id} not found.");
                }
                return updatedPokemon;
            }
            catch
            {
                throw new ApplicationException("An error occurred while updating the pokemon.");
            }
        }

        public async Task<bool> DeletePokemonAsync(string id)
        {
            try
            {
                var result = await _pokemonCollection.DeleteOneAsync(pokemon => pokemon.Id == id);
                if (result.DeletedCount == 0)
                {
                    throw new KeyNotFoundException($"Pokemon with Id {id} not found.");
                }
                return true;
            }
            catch
            {
                throw new ApplicationException("An error occurred while deleting the pokemon.");
            }
        }
    }
}
