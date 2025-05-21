using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

public class MoveRobot : MonoBehaviour
{
    [SerializeField] public GameObject J1_obj;
    [SerializeField] public GameObject J2_obj;
    [SerializeField] public GameObject J3_obj;
    [SerializeField] public GameObject J4_obj;
    public bool emergency_button;
    public bool emergency;

    private float J1;
    private float J2;
    private float J3;
    private float J4;

    private float J1speed;
    private float J2speed;
    private float J3speed;
    private float J4speed;

    private bool J1work;
    private bool J2work;
    private bool J3work;
    private bool J4work;

    bool J1MultiWork;
    bool J2MultiWork;
    bool J3MultiWork;
    bool J4MultiWork;

    private float curentJ1 = 0;
    private float curentJ2 = 0;
    private float curentJ3 = 0;
    private float curentJ4 = 0;

    private JToken points_data;
    private JToken speed_data;

    private string positionUrl;
    private string speedUrl;
    private string robot_readyUrl;
    private string motorsUrl;
    private string get_emergency;
    private string removePointUrl;
    private string removeSpeedUrl;
    private string setTriggerUrl;

    private float timer = 0;
    private bool is_start_multi_point = false;
    private int iterator_count = 0;

    public void update_urls()
    {
        string ip = PlayerPrefs.GetString("ip");
        string port = PlayerPrefs.GetString("port");

        if (ip == "") { ip = "ursystem.local"; }
        if (port == "") { port = "5000"; }

        positionUrl = "https://" + ip + ":" + port + "/get-position";
        speedUrl = "https://" + ip + ":" + port + "/get-speed";
        robot_readyUrl = "https://" + ip + ":" + port + "/set-ready";
        motorsUrl = "https://" + ip + ":" + port + "/set-motors-position";
        get_emergency = "https://" + ip + ":" + port + "/get-emergency";
        removePointUrl = "https://" + ip + ":" + port + "/remove-all-point-position";
        removeSpeedUrl = "https://" + ip + ":" + port + "/remove-all-point-speed";
        setTriggerUrl = "https://" + ip + ":" + port + "/set-position-id";
    }

    void Start()
    {
        update_urls();
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        var client = new HttpClient(handler);
        var values_position = new
        {
            robot =  "First",
            token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
        };
        var jsonContent = JsonConvert.SerializeObject(values_position);
        var content_str = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        try
        {
            var response = client.PostAsync(positionUrl, content_str).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            JToken json = JToken.Parse(responseString);

            J1 = float.Parse(json["data"]["J1"].ToString());
            J2 = float.Parse(json["data"]["J2"].ToString());
            J3 = float.Parse(json["data"]["J3"].ToString());
            J4 = float.Parse(json["data"]["J4"].ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }

        curentJ1 = -J1;
        curentJ2 = 90 - J2;
        curentJ3 = J3 - 90;
        curentJ4 = -J4;

        J1_obj.transform.localEulerAngles = new Vector3(0, 0, curentJ1);
        J2_obj.transform.localEulerAngles = new Vector3(curentJ2, 0, 0);
        J3_obj.transform.localEulerAngles = new Vector3(curentJ3, 0, 0);
        J4_obj.transform.localEulerAngles = new Vector3(curentJ4, 0, 0);
    }

    void Update()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        var client = new HttpClient(handler);
        if (emergency_button)
        {
            if (timer >= 1)
            {
                // Set motor position
                var values_motors_position = new 
                {
                    robot =  "First",
                    angles = new { J1 = -curentJ1, J2 = -(curentJ2 - 90), J3 = curentJ3 + 90, J4 = -curentJ4 },
                    token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                };
                var jsonContent = JsonConvert.SerializeObject(values_motors_position);
                var content_str = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                try
                {
                    client.PostAsync(motorsUrl, content_str);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                    
                }

                // Get Robot position
                var values_position = new
                {
                    robot =  "First",
                    token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                };
                jsonContent = JsonConvert.SerializeObject(values_position);
                content_str = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                try
                {
                    var response = client.PostAsync(positionUrl, content_str).Result;
                    var responseString = response.Content.ReadAsStringAsync().Result;

                    JToken json = JToken.Parse(responseString);

                    J1 = float.Parse(json["data"]["J1"].ToString());
                    J2 = float.Parse(json["data"]["J2"].ToString());
                    J3 = float.Parse(json["data"]["J3"].ToString());
                    J4 = float.Parse(json["data"]["J4"].ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                }

                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        if (!emergency_button)
        {
            if (emergency)
            {
                if (timer >= 1)
                {
                    // Set motor position
                    var values_motors_position = new
                    {
                        robot =  "First",
                        angles = new {J1= -curentJ1, J2 = -(curentJ2 - 90), J3 = curentJ3 + 90, J4 = -curentJ4},
                        token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                    };
                    var jsonContent = JsonConvert.SerializeObject(values_motors_position);
                    var content_str = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    try
                    {
                        client.PostAsync(motorsUrl, content_str);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Произошла ошибка: " + ex.Message);

                    }


                    var values_emergency = new
                    {
                        robot =  "First",
                        token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                    };
                    jsonContent = JsonConvert.SerializeObject(values_emergency);
                    var content_emerg = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    try
                    {
                        var response = client.PostAsync(get_emergency, content_emerg).Result;
                        var responseString = response.Content.ReadAsStringAsync().Result;
                        if (responseString == "True")
                        {
                            emergency = true;
                        }
                        else
                        {
                            emergency = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Произошла ошибка: " + ex.Message);
                    }

                    // Get Robot position
                    var values_position = new
                    {
                        robot =  "First",
                        token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                    };
                    jsonContent = JsonConvert.SerializeObject(values_position);
                    content_str = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    try
                    {
                        var response = client.PostAsync(positionUrl, content_str).Result;
                        var responseString = response.Content.ReadAsStringAsync().Result;

                        JToken json = JToken.Parse(responseString);

                        J1 = float.Parse(json["data"]["J1"].ToString());
                        J2 = float.Parse(json["data"]["J2"].ToString());
                        J3 = float.Parse(json["data"]["J3"].ToString());
                        J4 = float.Parse(json["data"]["J4"].ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Произошла ошибка: " + ex.Message);
                    }

                    timer = 0;

                }
                else
                {
                    timer += Time.deltaTime;
                }
            }
            else
            {
                if (timer >= 2)
                {
                    // set emergency
                    var values_emergency = new
                    {
                        robot =  "First",
                        token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                    };
                    var jsonContent = JsonConvert.SerializeObject(values_emergency);
                    var content_emerg = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    try
                    {
                        var response = client.PostAsync(get_emergency, content_emerg).Result;
                        var responseString = response.Content.ReadAsStringAsync().Result;
                        if (responseString == "True")
                        {
                            emergency = true;
                        }
                        else
                        {
                            emergency = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Произошла ошибка: " + ex.Message);
                    }
                }

                if (emergency)
                {

                }
                else
                {
                    if (timer >= 2)
                    {
                        // Set robot ready
                        if (J1work && J2work && J3work && J4work)
                        {
                            var values_robot_ready = new
                            {
                                robot =  "First",
                                state = true,
                                code =  "654123",
                                token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                            };
                            try
                            {
                                var jsonContent1 = JsonConvert.SerializeObject(values_robot_ready);
                                var content_str1 = new StringContent(jsonContent1, Encoding.UTF8, "application/json");
                                var resp = client.PostAsync(robot_readyUrl, content_str1).Result;
                                var responseString = resp.Content.ReadAsStringAsync().Result;
                                var parsed_state = Convert.ToBoolean(JToken.Parse(responseString)["data"]);
                                if (parsed_state == false)
                                {
                                    values_robot_ready = new
                                    {
                                        robot =  "First",
                                        state = false,
                                        code =  "654123",
                                        token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                                    };
                                    jsonContent1 = JsonConvert.SerializeObject(values_robot_ready);
                                    content_str1 = new StringContent(jsonContent1, Encoding.UTF8, "application/json");
                                    client.PostAsync(robot_readyUrl, content_str1);

                                    var new_values_robot_ready = new
                                    {
                                        robot =  "First",
                                        state = true,
                                        code =  "654123",
                                        token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                                    };
                                    jsonContent1 = JsonConvert.SerializeObject(values_robot_ready);
                                    content_str1 = new StringContent(jsonContent1, Encoding.UTF8, "application/json");
                                    client.PostAsync(robot_readyUrl, content_str1);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Произошла ошибка: " + ex.Message);
                            }
                        }
                        else
                        {
                            var values_robot_ready = new
                            {
                                robot =  "First",
                                state = false,
                                code =  "654123",
                                token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                            };
                            var jsonContent2 = JsonConvert.SerializeObject(values_robot_ready);
                            var content_str2 = new StringContent(jsonContent2, Encoding.UTF8, "application/json");
                            client.PostAsync(robot_readyUrl, content_str2);
                        }

                        // Set motor position
                        var values_motors_position = new
                        {
                            robot =  "First",
                            angles = new { J1 = -curentJ1, J2 = -(curentJ2 - 90), J3 = curentJ3 + 90, J4 = -curentJ4 },
                            token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                        };
                        var jsonContent = JsonConvert.SerializeObject(values_motors_position);
                        var content_str = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                        try
                        {
                            client.PostAsync(motorsUrl, content_str);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Произошла ошибка: " + ex.Message);

                        }

                        // Get Robot position
                        var values_position = new
                        {
                            robot =  "First",
                            token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                        };
                        jsonContent = JsonConvert.SerializeObject(values_position);
                        content_str = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                        try
                        {
                            var response = client.PostAsync(positionUrl, content_str).Result;
                            var responseString = response.Content.ReadAsStringAsync().Result;
                            if (JToken.Parse(responseString)["data"].ToString().Trim().StartsWith("{"))
                            {
                                // DEFAULT POINT AND SPEED
                                JToken json = JToken.Parse(responseString);
                                J1 = float.Parse(json["data"]["J1"].ToString());
                                J2 = float.Parse(json["data"]["J2"].ToString());
                                J3 = float.Parse(json["data"]["J3"].ToString());
                                J4 = float.Parse(json["data"]["J4"].ToString());

                                // Get Robot speed
                                var values_speed = new
                                {
                                    robot =  "First",
                                    token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                                };
                                var jsonContent3 = JsonConvert.SerializeObject(values_speed);
                                var content_str3 = new StringContent(jsonContent3, Encoding.UTF8, "application/json");
                                try
                                {
                                    var response_speed = client.PostAsync(speedUrl, content_str3).Result;
                                    var responseString_speed = response_speed.Content.ReadAsStringAsync().Result;
                                    JToken json_speed = JToken.Parse(responseString_speed);
                                    J1speed = float.Parse(json_speed["data"]["J1"].ToString());
                                    J2speed = float.Parse(json_speed["data"]["J2"].ToString());
                                    J3speed = float.Parse(json_speed["data"]["J3"].ToString());
                                    J4speed = float.Parse(json_speed["data"]["J4"].ToString());
                                }
                                catch (Exception ex)
                                {
                                    // Обработка ошибок
                                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                                }
                            }
                            else if (JToken.Parse(responseString)["data"].ToString().Trim().StartsWith("["))
                            {
                                if (!is_start_multi_point)
                                {
                                    is_start_multi_point = true;
                                    // MULTI POINT AND SPEED
                                    points_data = JToken.Parse(responseString)["data"];
                                    // Get Robot speed
                                    var values_speed = new
                                    {
                                        robot =  "First",
                                        token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                                    };
                                    var jsonContent3 = JsonConvert.SerializeObject(values_speed);
                                    var content_str3 = new StringContent(jsonContent3, Encoding.UTF8, "application/json");
                                    try
                                    {
                                        var response_speed = client.PostAsync(speedUrl, content_str3).Result;
                                        var responseString_speed = response_speed.Content.ReadAsStringAsync().Result;
                                        speed_data = JToken.Parse(responseString_speed);
                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        // Обработка ошибок
                                        print("Произошла ошибка: " + ex.Message);
                                    }
                                    // GO TO POINT FROM POINTS
                                    StartCoroutine(move());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Произошла ошибка: " + ex.Message);
                        }
                        timer = 0;
                    }
                    else
                    {
                        timer += Time.deltaTime;
                    }

                    // J1
                    if (curentJ1 - J1speed > -J1)
                    {
                        curentJ1 = curentJ1 - J1speed;
                        J1work = false;
                    }
                    else if (curentJ1 + J1speed < -J1)
                    {
                        curentJ1 = curentJ1 + J1speed;
                        J1work = false;
                    }
                    else
                    {
                        curentJ1 = -J1;
                        J1work = true;
                    }
                    J1_obj.transform.localEulerAngles = new Vector3(0, 0, curentJ1);

                    // J2
                    if (curentJ2 - J2speed > 90 - J2)
                    {
                        curentJ2 = curentJ2 - J2speed;
                        J2work = false;
                    }
                    else if (curentJ2 + J2speed < 90 - J2)
                    {
                        curentJ2 = curentJ2 + J2speed;
                        J2work = false;
                    }
                    else
                    {
                        curentJ2 = 90 - J2;
                        J2work = true;
                    }
                    J2_obj.transform.localEulerAngles = new Vector3(curentJ2, 0, 0);

                    // J3
                    if (curentJ3 - J3speed > J3 - 90)
                    {
                        curentJ3 = curentJ3 - J3speed;
                        J3work = false;
                    }
                    else if (curentJ3 + J3speed < J3 - 90)
                    {
                        curentJ3 = curentJ3 + J3speed;
                        J3work = false;
                    }
                    else
                    {
                        curentJ3 = J3 - 90;
                        J3work = true;
                    }
                    J3_obj.transform.localEulerAngles = new Vector3(curentJ3, 0, 0);

                    // J4
                    if (curentJ4 - J4speed > -J4)
                    {
                        curentJ4 = curentJ4 - J4speed;
                        J4work = false;
                    }
                    else if (curentJ4 + J4speed < -J4)
                    {
                        curentJ4 = curentJ4 + J4speed;
                        J4work = false;
                    }
                    else
                    {
                        curentJ4 = -J4;
                        J4work = true;
                    }
                    J4_obj.transform.localEulerAngles = new Vector3(curentJ4, 0, 0);
                }
            }
        } 
    }

    private IEnumerator move()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        var client = new HttpClient(handler);
        foreach (var point in points_data)
        {
            if (!emergency_button)
            {
                J1 = float.Parse(point["J1"].ToString());
                J2 = float.Parse(point["J2"].ToString());
                J3 = float.Parse(point["J3"].ToString());
                J4 = float.Parse(point["J4"].ToString());
                var send = point["send"].ToString();
                if (send != "")
                {
                    print(send);
                    var values_trigger_point = new
                    {
                        robot = "First",
                        id = send,
                        code = "654123",
                        token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                    };
                    var jsonContent1 = JsonConvert.SerializeObject(values_trigger_point);
                    var contP1 = new StringContent(jsonContent1, Encoding.UTF8, "application/json");
                    client.PostAsync(setTriggerUrl, contP1);
                }

                J1speed = float.Parse(speed_data["data"][iterator_count]["J1"].ToString());
                J2speed = float.Parse(speed_data["data"][iterator_count]["J2"].ToString());
                J3speed = float.Parse(speed_data["data"][iterator_count]["J3"].ToString());
                J4speed = float.Parse(speed_data["data"][iterator_count]["J4"].ToString());

                // null speed defense
                if (J1speed == 0) { J1speed = 1; }
                if (J2speed == 0) { J2speed = 1; }
                if (J3speed == 0) { J3speed = 1; }
                if (J4speed == 0) { J4speed = 1; }

                J1MultiWork = false;
                J2MultiWork = false;
                J3MultiWork = false;
                J4MultiWork = false;

                while (!J1MultiWork || !J2MultiWork || !J3MultiWork || !J4MultiWork)
                {
                    // J1
                    if (curentJ1 - J1speed > -J1)
                    {
                        curentJ1 = curentJ1 - J1speed;
                        J1MultiWork = false;
                    }
                    else if (curentJ1 + J1speed < -J1)
                    {
                        curentJ1 = curentJ1 + J1speed;
                        J1MultiWork = false;
                    }
                    else
                    {
                        curentJ1 = -J1;
                        J1MultiWork = true;
                    }
                    J1_obj.transform.localEulerAngles = new Vector3(0, 0, curentJ1);


                    // J2
                    if (curentJ2 - J2speed > 90 - J2)
                    {
                        curentJ2 = curentJ2 - J2speed;
                        J2MultiWork = false;
                    }
                    else if (curentJ2 + J2speed < 90 - J2)
                    {
                        curentJ2 = curentJ2 + J2speed;
                        J2MultiWork = false;
                    }
                    else
                    {
                        curentJ2 = 90 - J2;
                        J2MultiWork = true;
                    }
                    J2_obj.transform.localEulerAngles = new Vector3(curentJ2, 0, 0);

                    // J3
                    if (curentJ3 - J3speed > J3 - 90)
                    {
                        curentJ3 = curentJ3 - J3speed;
                        J3MultiWork = false;
                    }
                    else if (curentJ3 + J3speed < J3 - 90)
                    {
                        curentJ3 = curentJ3 + J3speed;
                        J3MultiWork = false;
                    }
                    else
                    {
                        curentJ3 = J3 - 90;
                        J3MultiWork = true;
                    }
                    J3_obj.transform.localEulerAngles = new Vector3(curentJ3, 0, 0);

                    // J4
                    if (curentJ4 - J4speed > -J4)
                    {
                        curentJ4 = curentJ4 - J4speed;
                        J4MultiWork = false;
                    }
                    else if (curentJ4 + J4speed < -J4)
                    {
                        curentJ4 = curentJ4 + J4speed;
                        J4MultiWork = false;
                    }
                    else
                    {
                        curentJ4 = -J4;
                        J4MultiWork = true;
                    }
                    J4_obj.transform.localEulerAngles = new Vector3(curentJ4, 0, 0);
                    yield return null;
                }

                iterator_count++;
                // REMOVE POINT AND SPEED IN SERVER
            }
            else
            {
                // Set motor position
                var values_motors_position = new 
                {
                    robot =  "First",
                    angles = new { J1 = -curentJ1, J2 = -(curentJ2 - 90), J3 = curentJ3 + 90, J4 = -curentJ4 },
                    token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                };
                var jsonContent2 = JsonConvert.SerializeObject(values_motors_position);
                var content_str2 = new StringContent(jsonContent2, Encoding.UTF8, "application/json");
                try
                {
                    client.PostAsync(motorsUrl, content_str2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                    
                }


                var values_emergency = new
                {
                    robot =  "First",
                    token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                };
                var jsonContent4 = JsonConvert.SerializeObject(values_emergency);
                var content_emerg = new StringContent(jsonContent4, Encoding.UTF8, "application/json");
                try
                {
                    var response = client.PostAsync(get_emergency, content_emerg).Result;
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    if (responseString == "True")
                    {
                        emergency = true;
                    }
                    else
                    {
                        emergency = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                }

                // Get Robot position
                var values_position = new
                {
                    robot =  "First",
                    token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
                };
                var jsonContent3 = JsonConvert.SerializeObject(values_position);
                var content_str3 = new StringContent(jsonContent3, Encoding.UTF8, "application/json");
                try
                {
                    var response = client.PostAsync(positionUrl, content_str3).Result;
                    var responseString = response.Content.ReadAsStringAsync().Result;

                    JToken json = JToken.Parse(responseString);

                    J1 = float.Parse(json["data"]["J1"].ToString());
                    J2 = float.Parse(json["data"]["J2"].ToString());
                    J3 = float.Parse(json["data"]["J3"].ToString());
                    J4 = float.Parse(json["data"]["J4"].ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                }
                break;
            }
            
        }
        iterator_count = 0;
        var values_delete_point = new
        {
            robot =  "First",
            code =  "654123",
            token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
        };
        var jsonContent = JsonConvert.SerializeObject(values_delete_point);
        var contP = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        client.PostAsync(removePointUrl, contP);

        var values_delete_speed = new 
        {
            robot =  "First",
            code =  "654123",
            token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
        };
        jsonContent = JsonConvert.SerializeObject(values_delete_speed);
        var contS = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        client.PostAsync(removeSpeedUrl, contS);
        

        var values_robot_ready = new
        {
            robot =  "First",
            state = true,
            code =  "654123",
            token = "6566a813a6564058077add5ed9d1e0d4d1da3c42d350b4d804b5999ed441f40f"
        };
        jsonContent = JsonConvert.SerializeObject(values_robot_ready);
        var content_str = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        client.PostAsync(robot_readyUrl, content_str);

        is_start_multi_point = false;
    }
}

