using System;
using System.Collections.Generic;
using System.Globalization;

namespace MealPlanner.Model
{
    /// <summary>
    /// Implementation of ICook. 
    /// </summary>
    public class Cook : ICook
    {
        private readonly Pantry pantry;
        private readonly List<IRecipe> recipeBook;

        public Cook(Pantry pantry)
        {
            this.pantry = pantry;
            recipeBook = new List<IRecipe>();
        }

        /// <summary>
        /// returns a read only list of loaded recipes.
        /// </summary>
        public IEnumerable<IRecipe> RecipeBook => recipeBook;

        /// <summary>
        /// Loads recipes from the files.
        /// Must parse the name, success rate, needed ingredients and
        /// necessary quantities.
        /// </summary>
        /// <param name="recipeFiles">Array of file paths</param>
        public void LoadRecipeFiles(string[] recipeFiles)
        {
            foreach (string file in recipeFiles)
            {
                string[] lines = System.IO.File.ReadAllLines(file);
                if (lines.Length < 2)
                    throw new ArgumentException("Invalid recipe file format.");

                string[] header = lines[0].Split(' ');
                string name = header[0].Trim();
                double successRate = double.Parse(header[1].Trim(), CultureInfo.InvariantCulture);

                Dictionary<IIngredient, int> ingredientsNeeded =
                    new Dictionary<IIngredient, int>();

                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(' ');
                    string ingredientName = parts[0].Trim();
                    int quantity = int.Parse(parts[1].Trim());

                    IIngredient ingredient = null;
                    foreach (IIngredient yummy in pantry.Ingredients)
                    {
                        if (yummy.Name.Equals(ingredientName, StringComparison.OrdinalIgnoreCase))
                        {
                            ingredient = yummy;
                            break;
                        }
                    }
                    if (ingredient == null)
                        ingredient = new Ingredient(ingredientName, "");

                    ingredientsNeeded.Add(ingredient, quantity);
                }

                IRecipe recipe = new Recipe(name, ingredientsNeeded, successRate);
                recipeBook.Add(recipe);
            }
        }

        /// <summary>
        /// Attempts to cook a meal from a given recipe. Consumes pantry 
        /// ingredients and returns the result message.
        /// </summary>
        /// <param name="recipeName">Name of the recipe to cook</param>
        /// <returns>A message indicating success, failure, or error</returns>
        public string CookMeal(string recipeName)
        {
            IRecipe selected = null;

            for (int i = 0; i < recipeBook.Count; i++)
            {
                if (recipeBook[i].Name.Equals(recipeName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    selected = recipeBook[i];
                    break;
                }
            }
            
            if (selected == null)
                return "Recipe not found.";

            foreach (KeyValuePair<IIngredient, int> needed in selected.IngredientsNeeded)
            {
                IIngredient ingredient = needed.Key;
                int need = needed.Value;
                int have = pantry.GetQuantity(ingredient);
                if (have < need)
                {
                    if (have == 0)
                        return "Missing ingredient: " + ingredient.Name;
         
                    return "Not enough " + ingredient.Name +
                           " (need " + need + ", have " + have + ")";
                }
            }

            foreach (KeyValuePair<IIngredient, int> needed in selected.IngredientsNeeded)
                if (!pantry.ConsumeIngredient(needed.Key, needed.Value))
                    return "Not enough ingredients";

            Random rng = new Random();
            if (rng.NextDouble() < selected.SuccessRate)
                return "Cooking '" + selected.Name + "' succeeded!";
            else
                return "Cooking '" + selected.Name + "' failed. Ingredients burned...";

        }
    }
}