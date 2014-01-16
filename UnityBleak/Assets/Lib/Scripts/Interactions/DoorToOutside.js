#pragma strict

private var OnDoor: boolean = false;

function Start () {

}

function OnTriggerEnter2D(other: Collider2D)
{
	OnDoor = true;
}

function Update () 
{
	if(OnDoor == true)
	{
		Application.LoadLevel("Scene 1-0.5");
	}
}