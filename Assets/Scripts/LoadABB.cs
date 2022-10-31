using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

/// <summary>
/// 加载AB包
/// </summary>
public class LoadABB : MonoBehaviour
{
    string path1 = "AssetBundles/cube.ab";
    string path2 = "AssetBundles/material.ab";
    void Start()
    {
        Fun1();
        //StartCoroutine(Fun2());
        //Fun3();
        //StartCoroutine(Fun4());
        //StartCoroutine(Fun5());
        //StartCoroutine(Fun7());
        //Fun8();
    }

    // 第一种方法
    public void Fun1(){
        // 加载材质，也可以说是加载prefeb的依赖
        AssetBundle material = AssetBundle.LoadFromFile(path2);
        AssetBundle ab = AssetBundle.LoadFromFile(path1);
        GameObject cubePrefeb = ab.LoadAsset<GameObject>("Cube");
        Instantiate(cubePrefeb);
    }

    // 第二种方法
    IEnumerator Fun2(){
        AssetBundleCreateRequest request1 = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path1));
        AssetBundleCreateRequest request2 = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path2));
        // 等待加载完成
        yield return request1;
        yield return request2;
        AssetBundle ab = request1.assetBundle;
        AssetBundle material = request2.assetBundle;
        GameObject cubePrefeb = ab.LoadAsset<GameObject>("Cube");
        Instantiate(cubePrefeb); 
    }

    // 第三种方法
    public void Fun3(){
        AssetBundle ab = AssetBundle.LoadFromMemory(File.ReadAllBytes(path1));
        AssetBundle material = AssetBundle.LoadFromMemory(File.ReadAllBytes(path2));
        GameObject cubePrefeb = ab.LoadAsset<GameObject>("Cube");
        Instantiate(cubePrefeb); 
    }

    // 第四种方法
    IEnumerator Fun4(){
        AssetBundleCreateRequest request1 = AssetBundle.LoadFromFileAsync(path1);
        AssetBundleCreateRequest request2 = AssetBundle.LoadFromFileAsync(path2);
        // 等待加载完成
        yield return request1;
        yield return request2;
        AssetBundle ab = request1.assetBundle;
        AssetBundle material = request2.assetBundle;
        GameObject cubePrefeb = ab.LoadAsset<GameObject>("Cube");
        Instantiate(cubePrefeb); 
    }

    // 第五种方法
    IEnumerator Fun5(){
        while(!Caching.ready){
            // 一帧暂停
            yield return null;
        }
        WWW www1 = WWW.LoadFromCacheOrDownload(@"file://C:\Users\ASUS\Desktop\unity\学习项目\AssetBundle\AssetBundles\cube.ab", 1);
        WWW www2 = WWW.LoadFromCacheOrDownload(@"file://C:\Users\ASUS\Desktop\unity\学习项目\AssetBundle\AssetBundles\material.ab", 1);
        yield return www1;
        yield return www2;
        AssetBundle ab = www1.assetBundle;
        AssetBundle material = www2.assetBundle;
        GameObject cubePrefeb = ab.LoadAsset<GameObject>("Cube");
        Instantiate(cubePrefeb); 
    }

    // 方法6
    IEnumerator Fun6(){
        while(!Caching.ready){
            // 一帧暂停
            yield return null;
        }
        WWW www1 = WWW.LoadFromCacheOrDownload(@"http://localhost/AssetBundles/cube.ab", 1);
        WWW www2 = WWW.LoadFromCacheOrDownload(@"http://localhost/AssetBundles/material.ab", 1);
        yield return www1;
        yield return www2;
        AssetBundle ab = www1.assetBundle;
        AssetBundle material = www2.assetBundle;
        GameObject cubePrefeb = ab.LoadAsset<GameObject>("Cube");
        Instantiate(cubePrefeb); 
    }

    // 方法7
    IEnumerator Fun7(){
        UnityWebRequest request1 = UnityWebRequestAssetBundle.GetAssetBundle(@"http://localhost:60470/AssetBundles/cube.ab");
        UnityWebRequest request2 = UnityWebRequestAssetBundle.GetAssetBundle(@"http://localhost:60470/AssetBundles/material.ab");
        yield return request1.SendWebRequest();
        yield return request2.SendWebRequest();
        AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request1);
        AssetBundle material = DownloadHandlerAssetBundle.GetContent(request2);
        GameObject cubePrefeb = ab.LoadAsset<GameObject>("Cube");
        Instantiate(cubePrefeb); 
    }

    // 方法8 ---通过manifest文件加载
    public void Fun8(){
        AssetBundle ab = AssetBundle.LoadFromFile(path1);
        GameObject cubePrefeb = ab.LoadAsset<GameObject>("Cube");
        Instantiate(cubePrefeb);

        AssetBundle manifestAB = AssetBundle.LoadFromFile("AssetBundles/AssetBundles");
        AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        // 得到Cube的依赖
        string[] strs = manifest.GetAllDependencies("Cube.ab");
        foreach (string str in strs)
        {
            print(str);
            AssetBundle.LoadFromFile("AssetBundles/" + str);
        }
    }
}
