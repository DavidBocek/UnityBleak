/*
-------------------------------
   Dialogue System for Unity
        Version 1.1.5

Copyright © 2013-2014 Pixel Crushers
-------------------------------


Thank you for buying the Dialogue System for Unity!

IMPORTANT: Upgrading? See the Upgrade Notes below!


Getting Started:
----------------
o Read the documentation (see below).
o Play the scene Assets/Dialogue System/Examples/Feature Demo/Feature Demo.


Documentation:
--------------
o Online documentation is accessible through the menu Window > Dialogue System > Help.
  A copy of the documentation is included in a zip file in the Documentation folder for offline use.
  Do NOT unzip it into your Assets folder! It contains Doxygen-generated JavaScript files; if these
  are in the Assets folder, Unity will try to compile them and fail. If you want an offline copy of
  the documentation, unzip it outside of your Assets folder.


Need Help?
----------
o Check the FAQ for solutions to common questions: Window > Dialogue System > Help > FAQ.
o Contact us any time via email or the web (see below).


Upgrade Notes:
--------------
o In version 1.1.4, the Examples/Scripts folder was moved to Scripts/Supplemental.
  If upgrading from 1.1.3 or earlier, please remove the Dialogue System folder before 
  importing this package.
o Re-import any Third Party Support packages that you imported in a previous release.
o 1.1.1: Daikon Forge UI code was refactored to use the new abstract base. If you use a custom
  DF-GUI UI, you may need to reassign GUI controls to its properties.
o 1.1.0: Common code shared by the Unity GUI, NGUI, and Daikon Forge UI systems was refactored
  to provide a more full-featured base for custom UI systems. The Unity Dialogue UI system was
  updated to use this new underlying code. If you use a custom Unity Dialogue UI prefab, you
  may need to reassign GUI controls to the properties on your dialogue UI component.
o 1.0.6: Chat Mapper 1.6 introduced several great new features, including validation.
  Unfortunately this means the Dialogue System can't repurpose the Video File field for cutscene 
  sequences. Use the custom Sequence field instead. All Chat Mapper projects and templates have been
  updated to use the Sequence field.


Support:
--------
Email: support@pixelcrushers.com
Web: Window > Dialogue System > Help > Report a Bug
*/