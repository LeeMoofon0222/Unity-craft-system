using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class craftanvil : MonoBehaviour
{
    [SerializeField] private BoxCollider Placeitemareacollider;
    [SerializeField] private ParticleSystem grayparticleSystem;
    [SerializeField] private ParticleSystem greenparticleSystem;
    [SerializeField] private ParticleSystem blueparticleSystem;
    [SerializeField] private ParticleSystem purpleparticleSystem;
    [SerializeField] private ParticleSystem orangeparticleSystem;
    [SerializeField] private ParticleSystem goldparticleSystem;
    public List<string> taglist;
    public List<string> outputlist;
    public Vector3 spawnpoint;
    public void Craft()
    {
        Debug.Log("Craft");
        taglist.Clear();
        outputlist.Clear();
        Collider[] colliderarray = Physics.OverlapBox(Placeitemareacollider.transform.position, Placeitemareacollider.size, Placeitemareacollider.transform.rotation);
        foreach (Collider collider in colliderarray)
        {
            if (!collider.transform.CompareTag("Untagged"))
            {
                taglist.Add(collider.transform.tag.ToString());
            }
        }
        Recipe();
        //只要合成東西就要重新點一次
        //最難合成的東西最好先顯示出來
        foreach (string str in outputlist)
        {
            if (str == "olive")
            {
                foreach (Collider tmd in colliderarray)
                {
                    if (tmd.transform.CompareTag("banana"))
                    {
                        Destroy(tmd);
                    }
                }
                Debug.Log("olive");
                Instantiate(GameObject.FindGameObjectWithTag(str), spawnpoint, Quaternion.identity);
                break;
            }
            if (str == "cherry")
            {
                foreach (Collider tmd in colliderarray)
                {
                    if(tmd.transform.CompareTag("hamburger") || tmd.transform.CompareTag("banana"))
                    {
                        Destroy(tmd);
                    }
                }
                Debug.Log("cherry");
                Instantiate(GameObject.FindGameObjectWithTag(str), spawnpoint, Quaternion.identity);
                break;
            }
            if (str == "hotdog")
            {
                foreach (Collider tmd in colliderarray)
                {
                    if (tmd.transform.CompareTag("cheese") || tmd.transform.CompareTag("banana"))
                    {
                        Destroy(tmd);
                    }
                }
                Debug.Log("hotdog");
                Instantiate(GameObject.FindGameObjectWithTag(str), spawnpoint, Quaternion.identity);
                break;
                
            }
            if (str == "hamburger")
            {
                foreach (Collider tmd in colliderarray)
                {
                    if (tmd.transform.CompareTag("olive") || tmd.transform.CompareTag("watermelon"))
                    {
                        Destroy(tmd);
                    }
                }
                Debug.Log("hamburger");
                Instantiate(GameObject.FindGameObjectWithTag(str), spawnpoint, Quaternion.identity);
                break;
            }
        }
    }
    public void Recipe()
    {
        if (taglist.Exists(t => t == "banana"))
        {
            taglist.Remove("banana");
            if (taglist.Exists(t => t == "banana"))
            {
                outputlist.Add("olive");
            }
            else
            {
                taglist.Add("banana");
            }
        }
        if (taglist.Exists(t => t == "hamburger") && taglist.Exists(t => t == "banana"))
        {
            outputlist.Add("cherry");
        }
        if (taglist.Exists(t => t == "cheese") && taglist.Exists(t => t == "banana"))
        {
            outputlist.Add("hotdog");
        }
        if (taglist.Exists(t => t == "olive") && taglist.Exists(t => t == "watermelon"))
        {
            outputlist.Add("hamburger");
        }
    }
}