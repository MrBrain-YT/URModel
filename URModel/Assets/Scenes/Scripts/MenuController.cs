using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    private bool settings_togle = false;
    public GameObject bases_togle;
    public GameObject ip_input_UI;
    public GameObject port_input_UI;
    public GameObject settings_panel;
    public GameObject basesCollector;
    void Start()
    {
        string ip = PlayerPrefs.GetString("ip");
        string port = PlayerPrefs.GetString("port");

        if (ip == "") { ip = "ursystem.local"; }
        if (port == "") { port = "5000"; }

        ip_input_UI.GetComponent<TMP_InputField>().text = ip;
        port_input_UI.GetComponent<TMP_InputField>().text = port;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void togle_settings()
    {
        if (settings_togle)
        {
            settings_togle = false;
            settings_panel.SetActive(false); 
        }
        else
        {
            settings_togle = true;
            settings_panel.SetActive(true);
        }
    }

    public void togle_bases_visible()
    {
        if (bases_togle.GetComponent<Toggle>().isOn)
        {
            basesCollector.SetActive(true);
        }
        else
        {
            basesCollector.SetActive(false);
        }
    }

    public void update_bases()
    {
        this.GetComponent<BasesManager>().update_bases();
    }

    public void go_to_ar()
    {
        SceneManager.LoadScene(1);
    }

    public void go_to_standart_scene()
    {
        SceneManager.LoadScene(0);
    }

    public void save_ip()
    {
        if (ip_input_UI.GetComponent<TMP_InputField>().text != "" && port_input_UI.GetComponent<TMP_InputField>().text != "")
        {
            PlayerPrefs.SetString("ip", ip_input_UI.GetComponent<TMP_InputField>().text);
            PlayerPrefs.SetString("port", port_input_UI.GetComponent<TMP_InputField>().text);
        }
        GameObject.FindGameObjectWithTag("robot").GetComponent<MoveRobot>().update_urls();
        this.GetComponent<emergency>().update_urls();
        this.GetComponent<BasesManager>().update_urls();
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void go_to_main_scene()
    {
        if (ip_input_UI.GetComponent<TMP_InputField>().text != "" && port_input_UI.GetComponent<TMP_InputField>().text != "")
        {
            PlayerPrefs.SetString("ip", ip_input_UI.GetComponent<TMP_InputField>().text);
            PlayerPrefs.SetString("port", port_input_UI.GetComponent<TMP_InputField>().text);
        }
        SceneManager.LoadScene(0);
    }
}
