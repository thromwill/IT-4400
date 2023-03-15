This C# WPF text editor is designed to offer a simple version of Windows Notepad.

![alt text](https://github.com/thromwill/INFOTC-4400/blob/main/TextEditor/TextEditor.png)

It includes the following features:

<pre>
TEXTBOX:
Large primary text box to edit .txt files.
</pre>
<pre>
OPTIONS:
  File - 
    New: create a new text file
    Open: open an existing text file
    Save: quitely save changes
    Save As: save changes, rename, and choose location
    Exit: exit the application
  Edit - 
    Undo: undo action
    Redo: redo action
    Cut: cut selected text
    Copy: copy selected text
    Paste: paste text from clipboard
    Select All: select all current text, including none if desired
  View - 
    Zoom In: increase zoom level by 10%
    Zoom Out: decrease zoom level by 10%
    Default: reset zoom level to 100%
    Fullscreen: toggle window size to full screen
    Text Wrapping: toggle text wrapping e.g. once edge is reached, text can go to next line or horizontal scroll bar can appear
  Help - 
    About: brief information about the application
</pre>
<pre>
SETTINGS:
  Font: change font family
  Font Style: select font style
  Font Weight: select font weight
  
  Links: LinkedIn, Github
  Help: brief information about the application
</pre>
<pre>
KEYBOARD INPUT:
Keyboard input is supported for various button presxes. Controls are listed in the options menus.
Use F3 to open the settings menu, F9 to toggle Word Wrap, and F11 to toggle fullscreen mode.
</pre>
<pre>
VALIDATION:
The text editor has been designed to validate any input scendario Examples include:
- Unavailable options are 'greyed out' and unclickable. For example, if there have been no changes to a 
  document Save becomes unavailable.
- The user is prompted to save their file if unsaved changes have been made and an unsaafe action, like closing
  the application, has been started.
</pre>
<Pre>
KNOWN BUGS:
  Pressing F3 multiple times in a row will leave the user stranded in the settings menu
</pre>
