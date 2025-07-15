using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [System.Serializable]
    public class CategoryData
    {
        public string name;
        public string logo;
        public string logoHover;
        public string type;
    }


    public class CategoryLoader : MonoBehaviour
    {
        public Transform contentParent;
        public GameObject itemPrefab;
        public bool IsSubCategory = false;

        public void Start()
        {
            if (IsSubCategory)
                LoadSubCategories();
            else
                LoadCategories();
        }

        public void LoadCategories()
        {
            try
            {
                TextAsset jsonText = Resources.Load<TextAsset>("Json/CategoryJson");
                List<CategoryData> categories = JsonUtilityWrapper.FromJson<CategoryData>(jsonText.text);

                foreach (var cat in categories)
                {
                    GameObject item = Instantiate(itemPrefab, contentParent);

                    item.GetComponentInChildren<TextMeshProUGUI>().text = cat.name;

                    Sprite logoSprite = Resources.Load<Sprite>(cat.logo);
                    Sprite hoverSprite = Resources.Load<Sprite>(cat.logoHover);

                    Image img = item.GetComponentInChildren<Image>();
                    if (logoSprite != null) img.sprite = logoSprite;

                    CategoryCardHover hoverHandler = item.GetComponentInChildren<CategoryCardHover>();

                    CategoryItemClick categoryClick = item.GetComponentInChildren<CategoryItemClick>();
                    categoryClick.CategoryName = cat.name;
                    categoryClick.sceneToLoad = "SubCategory";

                    if (hoverHandler != null)
                    {
                        hoverHandler.hoverSprite = hoverSprite;
                        hoverHandler.normalSprite = logoSprite;
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        private void LoadSubCategories()
        {


            var categoryName = SceneParams.categoryName;
            TextAsset jsonText = Resources.Load<TextAsset>($"Json/{SceneParams.categoryName}");
            List<CategoryData> categories = JsonUtilityWrapper.FromJson<CategoryData>(jsonText.text);

            foreach (var cat in categories)
            {
                GameObject item = Instantiate(itemPrefab, contentParent);

                Sprite logoSprite = Resources.Load<Sprite>(cat.logo);
                Sprite hoverSprite = Resources.Load<Sprite>(cat.logoHover);

                Image img = item.GetComponentInChildren<Image>();
                if (logoSprite != null) img.sprite = logoSprite;

                CategoryCardHover hoverHandler = item.GetComponentInChildren<CategoryCardHover>();
                if (hoverHandler != null)
                {
                    hoverHandler.hoverSprite = hoverSprite;
                    hoverHandler.normalSprite = logoSprite;
                }
            }
        }
    }
}