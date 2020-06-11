# Ringing Map C#
A project to create a map with every ringable set of bells on it.  This code was made to run in Visual Studio on Windows, and since switching to Linux, I have been unable to compile it so I will likely rewrite this in Rust in the future so that it is faster and more portable.  

The idea behind this project is to create a poster-style map containing every ringable tower.  In order to stand a chance of the tower names being visible, the map would be 1.5m by 1.5m and the text would be about 3mm tall (which is surprisingly legible).  If proper scaling was used, the map would be impossible because 95% of the towers are in a tiny fraction of the world's surface area (i.e. the UK).  So the map would be mainly England, with other places like America, Africa, Europe and Asia dotted around the outside where it fits.  However, this is still not quite big enough, and many places in England have too many towers to label even after cities are removed.  A pretty large re-think is required to get the labelling working, and the fact that I can't compile my existing code doesn't help.

## More Info & Screenshots


![Image of Yaktocat](https://raw.githubusercontent.com/Kneasle/ringing-map-c-sharp/master/Screenshots/Attempted%20Print.png)
