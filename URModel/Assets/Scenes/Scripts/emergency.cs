using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class emergency : MonoBehaviour
{
    private string set_emergency;
    //private string get_emergency = "https://localhost:5000/get-emergency";

    private void Start()
    {
        update_urls();
        GameObject.FindGameObjectWithTag("robot").transform.GetComponent<MoveRobot>().emergency_button = Convert.ToBoolean(PlayerPrefs.GetInt("emerg_button"));
    }

    public void update_urls()
    {
        string ip = PlayerPrefs.GetString("ip");
        string port = PlayerPrefs.GetString("port");

        if (ip == "") { ip = "ursystem.local"; }
        if (port == "") { port = "5000"; }
        set_emergency = "https://" + ip + ":" + port + "/set-emergency";
    }
    // Start is called before the first frame update
    public void enable_emerg()
    {
        bool emerg = GameObject.FindGameObjectWithTag("robot").transform.GetComponent<MoveRobot>().emergency_button;
        if (emerg)
        {
            GameObject.FindGameObjectWithTag("robot").transform.GetComponent<MoveRobot>().emergency_button = false;
            PlayerPrefs.SetInt("emerg_button", 0);
            // Set motor position
            var values_emergency = new
            {
                robot = "First",
                state = false,
                token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
            };
            var jsonContent3 = JsonConvert.SerializeObject(values_emergency);
            var content_str3 = new StringContent(jsonContent3, Encoding.UTF8, "application/json");
            try
            {
                var client = new HttpClient();
                var response_speed = client.PostAsync(set_emergency, content_str3).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }
        else
        {
            GameObject.FindGameObjectWithTag("robot").transform.GetComponent<MoveRobot>().emergency_button = true;
            PlayerPrefs.SetInt("emerg_button", 1);
            // Set motor position

            var values_emergency = new
            {
                robot = "First",
                state = true ,
                token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
            };
            var jsonContent3 = JsonConvert.SerializeObject(values_emergency);
            var content_str3 = new StringContent(jsonContent3, Encoding.UTF8, "application/json");
            try
            {
                var client = new HttpClient();
                var response_speed = client.PostAsync(set_emergency, content_str3).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }
    }
}
