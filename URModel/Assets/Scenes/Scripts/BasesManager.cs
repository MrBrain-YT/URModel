using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

public class BasesManager : MonoBehaviour
{
    public GameObject base_prefab;
    public GameObject bases_parent;
    private string get_bases_url;

    void Start()
    {
        update_urls();
        update_bases();
    }

    public void update_urls()
    {
        string ip = PlayerPrefs.GetString("ip");
        string port = PlayerPrefs.GetString("port");

        if (ip == "") { ip = "ursystem.local"; }
        if (port == "") { port = "5000"; }
        get_bases_url = "https://" + ip + ":" + port + "/get-bases";
    }

    // Update is called once per frame
    void Update()
    { 
    
    }
    public void update_bases() 
    {
        //_ = SendPostRequest();
        foreach (Transform child in bases_parent.transform)
        {
            Destroy(child.gameObject);
        }

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        var client = new HttpClient(handler);
        var values_position = new
        {
            robot = "First",
            token = "6c4a64cbfd8a92a5fb7444c5c2729c72c42037cf0e0343a7706ba297b3589763"
        };
        var jsonContent = JsonConvert.SerializeObject(values_position);
        var content_str = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        try
        {
            var response = client.PostAsync(get_bases_url, content_str).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            JToken json = JToken.Parse(responseString);

            var data = json["data"];
            if (data is JObject dataObject)
            {
                print(data);
                foreach (var property in dataObject.Properties())
                {
                    string key = property.Name;
                    JToken value = property.Value;

                    // Предположим, что value["x"], value["y"], value["z"] содержат числа
                    float x = value["x"].Value<float>() / 100f;
                    float y = value["y"].Value<float>() / 100f;
                    float z = value["z"].Value<float>() / 100f;

                    float a = value["a"].Value<float>();
                    float b = value["b"].Value<float>();
                    float c = value["c"].Value<float>();

                    GameObject _base = Instantiate(base_prefab, parent:bases_parent.transform);
                    _base.transform.localPosition = new Vector3(x, z, y);
                    _base.transform.localEulerAngles = new Vector3(c, a, b);
                    _base.name = key;
                }
            }
            else
            {
                Console.WriteLine("Data is not a dictionary.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }
}
