doc = app.activeDocument;
$.writeln("-----------------------------------------------------");
 
var totalLayers = doc.layers.length;

for( var v  = 0; v <  totalLayers; v++){

    for(var c = 0; c < doc.layers[v].pathItems.length; c++)
    {
          $.write(doc.layers[v].pathItems[c].name);
           $.write(" ");
          $.write(doc.layers[v].pathItems[c].position);
          $.writeln();
    }
    
 

 }
       
    