# Ringing Map C#
A project to create a map with every ringable set of bells on it.  This code was made to run in Visual Studio on Windows, and since switching to Linux I have been unable to compile it so I will likely rewrite this in Rust in the future so that it is faster and more portable.  

The idea behind this project is to create a poster-style map containing every ringable tower.  In order to stand a chance of the tower names being visible, the map would be 1.5m by 1.5m and the text would be about 3mm tall (which is surprisingly legible).  If the entire world was put to scale, the map would be impossible because 95% of the towers are in a tiny fraction of the world's surface area (i.e. the UK).  So the map would be mainly England, with other places like America, Africa, Europe and Asia dotted around the outside where it fits.  However, this is still not quite big enough, and many places in England have too many towers to label even after cities are removed.  A pretty large re-think is required to get the labelling working, and the fact that I can't compile my existing code doesn't help.

## More Info & Screenshots
# Data acquisition and filtering
The first part of the project was to procure the data required to make the maps.  For the coastlines, I used the snappily named [Global Self-consistent, Hierarchical, High-resolution Geography Database](http://www.soest.hawaii.edu/pwessel/gshhg/), and for the towers of course there's [Dove's Guide](https://dove.cccbr.org.uk/).  With some preprocessing in Python to handle things like filtering out the vast numbers of coastlines not needed in the map and turning Lat/Long coords into OS grid coordinates to prevent the UK from getting really squished, the coastline and tower data was cached for C# to turn into the map.

# Point design
Now came the real challenge of making a map with 7000+ items on it, and having the result be easily parsable by a human.  I tried many methods of labelling, but the one I settled on eventually was to exploit the observation that there are no towers on Dove's with 7, 9 or 11 bells since these are all rounded down to the nearest even number.  Therefore, our label design only had to deal with 3, 4, 5, 6, 8, 10 and 12 bells (with 4 towers of 15 or 16 which could be handled separately.  The final design was to use polygons who's total number of sides was the number of bells (so for example, two pentagons inside each other corresponds to a 10).  This works fantastically, because the most complex shape is a hexagon, no colour information is used so unringable towers can be grey, and it's easily legible even when the points are about 2mm in size.

![Numbering example](https://raw.githubusercontent.com/Kneasle/ringing-map-c-sharp/master/Screenshots/Tower%20numbering.png)

# Some obvious issues
Two issues became apparent early on in the design (outside the the sheer enourmity of the project).  First issue is that the naming system used thus far (just using the place name from Dove's) was not going to cut it when it came to cities or towns, since you'll get a whole swathe of identically labelled towers, which is not useful (shown in the screenshot below).

![Intelligent naming is required](https://raw.githubusercontent.com/Kneasle/ringing-map-c-sharp/master/Screenshots/Intelligent%20naming%20required.png)

However, you cannot just filter for unique tower names because many towers share common place names but are many miles apart (for example, the name of my home tower, Stone, is shared by at least 2 other ringable towers).  So a distance cutoff was made for when two towers of the same name had to be labelled using the dedication instead of the place name.  This in general worked really nicely, and a bit of geographic intuition goes a long way to figuring out the place names.

Another issue is that of cities.  The map will currently display cities as a blob of towers all very close together which are impossible to label (although our point design is working very well because the bell numbers are still identifiable):

![Cities](https://raw.githubusercontent.com/Kneasle/ringing-map-c-sharp/master/Screenshots/City%20detection%20required.png)

My intended solution to this (which never got fully implemented before switching to Linux) was to find the cities, remove them from the map, and display them enlarged round the edge of the map.  The first bit, the border detection, was implemented, first with some strange results:

![Broken borders](https://raw.githubusercontent.com/Kneasle/ringing-map-c-sharp/master/Screenshots/City%20Borders%20Not%20Quite%20Working.png)

and then with some tweaking and bug-fixing, here showing what I belive is the border around London before the coastline data was added:

![Fixed border around London](https://github.com/Kneasle/ringing-map-c-sharp/blob/master/Screenshots/The%20border%20around%20London.png)

# TODO

The screenshot here also includes a bold coastline on the right, and a diocese border along the top left which were also added early on.

![Screenshot of some towers](https://raw.githubusercontent.com/Kneasle/ringing-map-c-sharp/master/Screenshots/Region%20Example.png)

