# Unity Movement Script v1.5
 A first person movement script made for unity 3D

Unity Movement Script v1.5

This is the most recent version of my first take on a rigidbody first person movement script. I developed for testing, studying purposes and I planned to eventually use it in a project. But my focus shifted and I am developing a completely new script that is more advanced and easier to implement in the future. I don't plan on further updating it. But this might be of use for someone.

This script has some functions that took me awhile to me to develop that include the capacity of clibling staircases, the rigidbody not slipping down slopes or inclines and some easier function like sliding and croutching. I didn't implement a button for sprinting because it wasn't necessary but should be rather ease to implement.

For the rigidbody to move it is necessary to set the layers of the ground to 'Normal' or 'Step', for then to climb the object need to use the layer of 'step' and 
if you wish to implement a incline to steep you need to use the layer 'Slopes' on it.
