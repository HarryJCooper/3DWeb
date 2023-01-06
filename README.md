# 3DWeb
An attempt at making a native feeling VR web browser that feels native with Unity and OpenXR
## How it works
* Hit a URL
* Parse the HTML (using AngleSharp)
* Create game objects corresponding to each node of the DOM
* Distribute them in a three dimensional fashion

## Next steps
* Allow for resizing of 'nodes'
* Allow for multiple sites to load when opening the app, creating a 'home area', rather than a 'homepage'.
* Run JS using C# JS engine
* Make things clickable!
* Allow user to move within the space
