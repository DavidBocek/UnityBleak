// Attach this to a GUIText to make a frames/second indicator.

//

// It calculates frames/second over each updateInterval,

// so the display does not keep changing wildly.

//

// It is also fairly accurate at very low FPS counts (<10).

// We do this not by simply counting frames per interval, but

// by accumulating FPS for each frame. This way we end up with

// correct overall FPS even if the interval renders something like

// 5.5 frames.

 

var updateInterval = 2.0;

 

private var accum = 0.0; // FPS accumulated over the interval

private var frames = 0; // Frames drawn over the interval

private var timeleft : float; // Left time for current interval

 

private var show = false;

private var text : String;

 

function Start()

{

    if( !guiText )

    {

        print ("FramesPerSecond needs a GUIText component!");

        enabled = false;

        return;

    }

    timeleft = updateInterval;  

}

 

function Update()

{

    if (Input.GetKeyDown ("1"))

        show = !show;

        

    timeleft -= Time.deltaTime;

    accum += 1.0/Time.deltaTime;

    ++frames;

    

    // Interval ended - update GUI text and start new interval

    if( timeleft <= 0.0 )

    {

        text = "" + (accum/frames).ToString("f1");

        timeleft = updateInterval;

        accum = 0.0;

        frames = 0;

    }

    

    guiText.text = show ? text : "";

}