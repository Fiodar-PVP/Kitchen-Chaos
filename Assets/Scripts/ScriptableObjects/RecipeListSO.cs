using System.Collections.Generic;
using UnityEngine;

//Disable Asset Menu since we've already created RecipeListSO and we only need 1 RecipeListSO ever
//[CreateAssetMenu()]
public class RecipeListSO : ScriptableObject
{
    public List<RecipeSO> recipeSOList;
}
