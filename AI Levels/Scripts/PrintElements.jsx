doc = app.activeDocument;
$.writeln("-----------------------------------------------------");
 
var totalLayers = doc.layers.length;

var layerWidth = doc.width;
var layerHeight = doc.height;



var centreX = layerWidth / 2;
var centreY = layerHeight / 2;

$.writeln(centreX + " " + centreY);


for( var v  = 0; v <  totalLayers; v++){

    for(var c = 0; c < doc.layers[v].pathItems.length; c++)
    {
        // name, posX, posY
        
        $.write(doc.layers[v].name + ",");
        $.write(doc.layers[v].pathItems[c].name);
        $.write(",");
           
           var pItem = doc.layers[v].pathItems[c];
           var x = doc.layers[v].pathItems[c].position[0];
           var y = doc.layers[v].pathItems[c].position[1];
           
           y  = y * -1; // times by -1 to get postive 
       
           x = x  + pItem.width / 2;
           y = y + pItem.height / 2;
            
           var posX = x - layerWidth / 2; 
           var posY = y - layerHeight / 2;
   

           $.write( posX + ","  + posY);
           
           // now we need to create a distance from orgin value to the centre of the object 
           
           //$.write(doc.layers[v].pathItems[c].position); // this gives up the top left corner so we need to get the centre (as we draw from centre in game)
          $.writeln();
    }
    
 

 }
       
    