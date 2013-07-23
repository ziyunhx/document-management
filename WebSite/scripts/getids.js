function selectAll(form)
{
    obj = document.forms[form];
	for(var i = 0;i<obj.length;i++)
	{
		if(obj.elements[i].type == "checkbox" )
		{
			//if(obj.elements[i].name!="selAll")
			//{
				if(!obj.elements[i].checked)
					obj.elements[i].checked = true;
				else
					obj.elements[i].checked = false;
			//}
		}
	}
}

function getSelectedValue(form)
{
	var ids = '';
	obj = document.forms[form];
	for(var i = 0;i<obj.length;i++)
	{
		if(obj.elements[i].type == "checkbox" )
		{
			if(obj.elements[i].name!="selAll")
			{
				if(obj.elements[i].checked)
					ids += obj.elements[i].value + ',';
			}
		}
	}
	if(ids!='')
	{
		ids = ids.substr(0,ids.length-1);
	}
	return ids;
}

function isValidEmail(s)
{
	var reg1 = new RegExp('^[a-zA-Z0-9][a-zA-Z0-9@._-]{3,}[a-zA-Z]$');
	var reg2 = new RegExp('[@.]{2}');
	
	if (s.search(reg1) == -1
			|| s.indexOf('@') == -1
			|| s.lastIndexOf('.') < s.lastIndexOf('@')
			|| s.lastIndexOf('@') != s.indexOf('@')
			|| s.search(reg2) != -1)
		return false;
	
	return true;
}

function Mousin(id)
{
    var obj=document.getElementById(id.id)
    obj.style.background=(obj.style.background="FFFFFF"?"FF0000":"FFFFFF");
}
function Mousout(id)
{
    var obj=document.getElementById(id.id)
    obj.style.background=(obj.style.background="FFFFFF"?"FFFFFF":"FF0000");
}