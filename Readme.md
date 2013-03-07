# Cupboard

Cupboard is a post build tool for making it easy to manage client side templates during the development process, yet easily have them available in JavaScript. It lets you manage your client side templates as individual files, then builds a JavaScript file that can be used on the client side. 

Imagine the following directory structure with each html file containing an individual template.

```
templates
	home.html
    users
        detail.html
		listItem.html
        new.html
    posts
        detail.html
        edit.html
        listItem.html
        new.html
```

Cupboard would scan this folder and create a JavaScript file like:

```JavaScript```
var templates = {
	home: 'The contents of home.html',
	users: {
		detail: 'The contents of users/detail.html'
		listItem: 'The contents of users/listItem.html'
		new: 'The contents of users/new.html'
	},
	posts: {
		detail: 'The contents of posts/detail.html'
		edit: 'The contents of posts/edit.html'
		listItem: 'The contents of posts/listItem.html'
		new: 'The contents of posts/new.html'
	}
};
```

Optionally, Cupboard can run the underscore/lodash _.template() function on the templates.

For example: 
```
	detail: _.template('The contents of users/detail.html')
```

# Installation

[Download version 0.1.0 as a zip file](https://github.com/craigmaslowski/cupboard/blob/master/binaries/0.1.0/Cupboard.0.1.0.zip?raw=true) 

Or install with Chocolatey:

```
cinst cupboard
```

# Usage

Add a post build event to your Web project similar to the following. Be sure to replace the source and destination parameter with the appropriate paths. Also make sure that the folder where cupboard.exe is located is in your path, or provide an absolute path to it.

```
cupboard.exe -s $(ProjectDir)ClientSideTemplates -d  $(ProjectDir)Content\Scripts\templates.js
```

# Command Line Options

* -s, -source : The location of the templates folder. This is a **required parameter**.
* -d, -destination : Final destination of the compiled template file. This is a **required parameter**.
* -e, -extension : File extension used for templates. Default: html
* -v, -variable : Name of JavaScript variable that stores the template hash. Default: templates
* -c, -compile : Wrap contents of template files with call to _.template(). Default: false