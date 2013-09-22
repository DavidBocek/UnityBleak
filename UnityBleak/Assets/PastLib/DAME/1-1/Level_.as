//Code generated with DAME. http://www.dambots.com

package com
{
	import org.flixel.*;
	public class Level_ extends BaseLevel
	{
		//Embedded media...
		[Embed(source="../../Map Assets/1-1/tilemap/mapCSV_Group1_Map1.csv", mimeType="application/octet-stream")] public var CSV_Group1Map1:Class;
		[Embed(source="../../Map Assets/1-1/tilemap/tilesFINALmergedUSEFINISHED13012 - Copy.png")] public var Img_Group1Map1:Class;

		//Tilemaps
		public var layerGroup1Map1:FlxTilemap;

		//Sprites
		public var Group1Layer2Group:FlxGroup = new FlxGroup;
		public var Group1Layer3Group:FlxGroup = new FlxGroup;


		public function Level_(addToStage:Boolean = true, onAddSpritesCallback:Function = null)
		{
			// Generate maps.
			layerGroup1Map1 = new FlxTilemap;
			layerGroup1Map1.loadMap( new CSV_Group1Map1, Img_Group1Map1, 44,44, FlxTilemap.OFF, 0, 1, 1 );
			layerGroup1Map1.x = -924.000000;
			layerGroup1Map1.y = 1232.000000;
			layerGroup1Map1.scrollFactor.x = 1.000000;
			layerGroup1Map1.scrollFactor.y = 1.000000;

			//Add layers to the master group in correct order.
			masterLayer.add(Group1Layer2Group);
			masterLayer.add(Group1Layer3Group);
			masterLayer.add(layerGroup1Map1);


			if ( addToStage )
			{
				addSpritesForLayerGroup1Layer2(onAddSpritesCallback);
				addSpritesForLayerGroup1Layer3(onAddSpritesCallback);
				FlxG.state.add(masterLayer);
			}

			mainLayer = layer;

			boundsMinX = -924;
			boundsMinY = 1232;
			boundsMaxX = 6820;
			boundsMaxY = 2376;

		}

		override public function addSpritesForLayerGroup1Layer2(onAddCallback:Function = null):void
		{
			addSpriteToLayer(Avatar, Group1Layer2Group , 120.000, -126.500, 0.000, false, 1, 1, onAddCallback );//"pictures"
		}

		override public function addSpritesForLayerGroup1Layer3(onAddCallback:Function = null):void
		{
			addSpriteToLayer(Avatar, Group1Layer3Group , -742.000, 1524.000, 0.000, false, 1, 1, onAddCallback );//"crab"
			addSpriteToLayer(Avatar, Group1Layer3Group , -153.000, 1656.000, 0.000, false, 1, 1, onAddCallback );//"crab"
			addSpriteToLayer(Avatar, Group1Layer3Group , 1512.000, 1480.000, 0.000, false, 1, 1, onAddCallback );//"crab"
			addSpriteToLayer(Avatar, Group1Layer3Group , 2645.000, 1481.000, 0.000, false, 1, 1, onAddCallback );//"crab"
			addSpriteToLayer(Avatar, Group1Layer3Group , 5541.000, 1482.000, 0.000, false, 1, 1, onAddCallback );//"crab"
			addSpriteToLayer(Avatar, Group1Layer3Group , 682.000, 1611.000, 0.000, false, 1, 1, onAddCallback );//"crab"
			addSpriteToLayer(Avatar, Group1Layer3Group , 1294.000, 1913.000, 0.000, false, 1, 1, onAddCallback );//"broken_crab"
			addSpriteToLayer(Avatar, Group1Layer3Group , 1206.000, 1780.000, 0.000, false, 1, 1, onAddCallback );//"broken_crab"
			addSpriteToLayer(Avatar, Group1Layer3Group , 4737.000, 1471.000, 0.000, false, 1, 1, onAddCallback );//"broken_crab"
			addSpriteToLayer(Avatar, Group1Layer3Group , 2139.000, 2053.000, 0.000, false, 1, 1, onAddCallback );//"crawling_death"
			addSpriteToLayer(Avatar, Group1Layer3Group , 4532.000, 1960.000, 0.000, false, 1, 1, onAddCallback );//"crawling_death"
			addSpriteToLayer(Avatar, Group1Layer3Group , 5835.000, 1963.000, 0.000, false, 1, 1, onAddCallback );//"crawling_death"
		}


	}
}
