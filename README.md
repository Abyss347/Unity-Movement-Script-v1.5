# Unity Movement Script v1.5
 A first person movement script made for unity 3D


Unity Movement Script v1.5

This is the most recent version of my first take on a rigidbody first person movement script. I developed for testing, studying purposes and I planned to eventually use it in a project. But my focus shifted and I am developing a completely new script that is more advanced and easier to implement in the future. I don't plan on further updating it. But this might be of use for someone.

This script has some functions that took me awhile to me to develop that include the capacity of clibling staircases, the rigidbody not slipping down slopes or inclines and some easier function like sliding and croutching. I didn't implement a button for sprinting because it wasn't necessary but should be rather ease to implement.

For the rigidbody to move it is necessary to set the layers of the ground to 'Normal' or 'Step', for then to climb the object need to use the layer of 'step' and 
if you wish to implement a incline to steep you need to use the layer 'Slopes' on it.

MIT License

Copyright (c) 2023 Vitor Moretti SIa

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
