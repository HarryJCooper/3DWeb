using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using AngleSharp;
using AngleSharp.Dom;
using System.Text.RegularExpressions;

// COULD HAVE CERTAIN VR TAGS OR ATTRIBUTES
// A VR ATTRIBUTE COULD HAVE X/Y/Z POSITION AND ROTATION, BECAUSE IT DOESN'T NEED TO SUPPORT MULTIPLE SCREEN SIZES, THE LAYOUT STEP CAN BE MUCH SIMPLIFIED
// IT COULD BE ATTACHED TO THE PARENT, SO YOU'D USE HTML TAGS AS THE HIERARCHY, AND THEN IF THEY HAD A VR-PARENT TAG IT WOULD CREATE A PARENT?
// WHAT OTHER ATTRIBUTES WORK?

// TODO - Make a socket interactor at initial placement of words
// Add Raycast interactor to pull words closer
// Add Movement to move freely in space
// Add Recentering to reset user to 0/0/0
// Add world movement to expand/reduce all items
// Make things clickable

public class ReturnHTML : MonoBehaviour
{
    public string address = "";
    string homeAddress = "https://purplejamaudio.com";
    public GameObject[] tmpGameObjects;
    public GameObject tmpGameObject, environment, parentGameObject;
    public string[] noneRenderedTags = {"head","meta","script","html"};
    public string[] exclusions = {"   ", "\t"};
    List<GameObject> youHavePassedTheTest = new List<GameObject>();
    List<GameObject> links = new List<GameObject>();
    public List<GameObject> parentGameObjects = new List<GameObject>();
    List<string> selectors = new List<string>();
    int distanceFromObserver = 20;

    void Start(){
        if (address == "") address = homeAddress;
        GetText();
    }

    void CreateEl(string textContent, string localName, int zPos){
        // CHECKING IF THE TEXT CONTENT IS THE SAME AS PREVIOUS ELEMENT, IF SO DON'T RENDER
        if (youHavePassedTheTest.Count > 0 
        && youHavePassedTheTest[youHavePassedTheTest.Count - 1].transform.GetChild(0).GetComponent<TextMeshPro>().text == textContent) return;
        
        // CHECK IF HTML STRUCTURE INDICATES A NEW SECTION
        if (zPos < 5){
            Debug.Log(textContent);
            GameObject parent = Instantiate(parentGameObject, environment.transform);
            parentGameObjects.Add(parent);
            parent.transform.Rotate(0, (22.5f*parentGameObjects.Count), 0);
        }

        // CREATE NEW ELEMENT CHILDED TO THE LATEST PARENT
        GameObject newEl = Instantiate(tmpGameObject, parentGameObjects[parentGameObjects.Count - 1].transform);
        youHavePassedTheTest.Add(newEl);

        if (localName == "a"){
            links.Add(newEl);
        }
        
        // SET TEXT CONTENT
        TextMeshPro tmp = newEl.transform.GetChild(0).GetComponent<TextMeshPro>();
        tmp.text = textContent;

        if (localName == "p") tmp.fontSize = 16;
        else if (localName == "div") tmp.fontSize = 16;
        else if (localName == "h1") tmp.fontSize = 48;
        else if (localName == "h2") tmp.fontSize = 40;
        else if (localName == "h3") tmp.fontSize = 32;
        else if (localName == "h4") tmp.fontSize = 24;
        else tmp.fontSize = 16;

        newEl.transform.localPosition = new Vector3(0, parentGameObjects[parentGameObjects.Count - 1].transform.childCount * -1, zPos + distanceFromObserver);
    }

    async void GetText(){
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = context.OpenAsync(address).Result;
        var elements = document.QuerySelectorAll("*");
    
        for (int i = 0; i < elements.Length; i++){
            bool notRender = false;
            foreach(string tag in noneRenderedTags) {
                if (elements[i].LocalName == tag) {
                    notRender = true;
                    break;
                }
            }
            if (notRender) continue;

            var textContent = elements[i].TextContent.TrimStart().TrimEnd();
            if (textContent == "") continue;
            
            foreach(string exclusion in exclusions) {
                if (textContent.Contains(exclusion) && elements[i].LocalName != "a"){
                    notRender = true;
                    break;
                }
            }
            if (notRender) continue;
            
            string selector = ElementExtensions.GetSelector(elements[i]);

            int zPos = selector.Split('>').Length - 1;

            Debug.Log(selector);

            CreateEl(textContent, elements[i].LocalName, zPos);
        }
    }
}
