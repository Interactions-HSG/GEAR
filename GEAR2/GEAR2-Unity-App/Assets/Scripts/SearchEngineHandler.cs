using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using wot_td_csharp;


public class SearchEngineHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void ExecuteHTTPRequest(ThingDescription td)
    //public void ExecuteHTTPRequest(ThingDescription td)
    {
        /*executing HTTP Requests*/
        List<string>? propertyNames = td.GetPropertyNames();
        if (propertyNames != null)
        {
            // get a property by name
            PropertyAffordance? property1 = td.GetPropertyByName(propertyNames[0]);

            //*
            foreach (var properName in propertyNames)
            {
                print(properName + "\n");
            }
            //*/

            //*
            if (property1 != null)
            {
                // get the property value from the thing
                string response = await property1.GetPropertValue();
                print("Get Response: " + response + "\n");
                // set the property value
                string response2 = await property1.SetPropertyValue("on");
                print("Set Response: " + response2 + "\n");
            }
            //*/
        }
    }

    public void ShowFeedback()
    {
        //ThreeDModelContainer.SetActive(true);

	// CHANGE this URI to point to the correct Thing Description!
        String uri = "http://127.0.0.1"; 

        ThingDescription td = TDGraphReader.ReadFromUri(new Uri(uri), TDFormat.jsonld);
        //ThingDescription td = TDGraphReader.ReadFromFile(filePath, TDFormat.jsonld);

        print("SearchEngineHandler will show some feedback. . .  ");

        ExecuteHTTPRequest(td);

    }
}
