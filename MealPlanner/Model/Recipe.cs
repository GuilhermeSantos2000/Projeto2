using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlanner.Model
{
    public class Recipe : IRecipe
    {
        public string Name { get; }
        public IReadOnlyDictionary<IIngredient, int> IngredientsNeeded { get; }
        public double SuccessRate { get; }

        public Recipe(string name, Dictionary<IIngredient, int> ingredientsNeeded, double successRate)
        {
            Name = name;
            IngredientsNeeded = ingredientsNeeded;
            SuccessRate = successRate;
        }

        public int CompareTo(IRecipe other)
        {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }
}