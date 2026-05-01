using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System.Collections;
using System.Collections.Generic;

public class AddressableTest : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject obj;
    [SerializeField] private AssetReferenceGameObject[] objects;

    [SerializeField] private AssetReferenceSprite sprite;
    [SerializeField] private Image flagImage;

    private List<GameObject> objectList = new();

    // AssetReferenceT<AudioClip> : 오디오 / private GameObject BGMobj

    private void Start()
    {
        StartCoroutine(InitAddreassable());
    }

    private IEnumerator InitAddreassable()
    {
        var init = Addressables.InitializeAsync();
        yield return init;
    }

    public void SpawnObject()
    {
        obj.InstantiateAsync().Completed += (obj) =>
        {
            objectList.Add(obj.Result);
        };

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].InstantiateAsync().Completed += (obj) =>
            {
                objectList.Add(obj.Result);
            };
        }

        sprite.LoadAssetAsync().Completed += (img) =>
        {
            var image = flagImage.GetComponent<Image>();
            image.sprite = img.Result;
        };
    }

    public void ReleaseObject()
    {
        foreach (GameObject obj in objectList)
        {
            if (obj != null)
            {
                Addressables.ReleaseInstance(obj);
            }
        }
        objectList.Clear();

        if (flagImage != null)
        {
            flagImage.sprite = null;
        }
        sprite.ReleaseAsset();
    }
}
