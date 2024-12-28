<style>
	.demo-card-wide.mdl-card {
		width: 100%;
		min-height: inherit;
		padding: 15px;
		min-width: 800px;
	}

	.demo-card-wide>.mdl-card__menu {
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
					Windows systems: <?php echo mysqli_query($conn, "SELECT COUNT(*) FROM command WHERE `system` = '" . OS_WINDOWS . "'")->fetch_column(); ?>
				</span>
			</li>
			<li class="mdl-list__item">
				<i class="fab fa-linux"></i>
				<span class="mdl-list__item-primary-content">
					Linux systems (*): <?php echo mysqli_query($conn, "SELECT COUNT(*) FROM command WHERE `system` = '" . OS_LINUX . "'")->fetch_column(); ?>
				</span>
			</li>
			<li class="mdl-list__item">
				<i class="fab fa-apple"></i>
				<span class="mdl-list__item-primary-content">
					macOs systems (*): <?php echo mysqli_query($conn, "SELECT COUNT(*) FROM command WHERE `system` = '" . OS_MACOS . "'")->fetch_column(); ?>
				</span>
			</li>
		</ul>
		<h5>Mobile</h5>
		<ul class="demo-list-item mdl-list">
			<li class="mdl-list__item">
				<i class="fab fa-android"></i>
				<span class="mdl-list__item-primary-content">
					Android systems (*): <?php echo mysqli_query($conn, "SELECT COUNT(*) FROM command WHERE `system` = '" . OS_ANDROID . "'")->fetch_column(); ?>
				</span>
			</li>
			<li class="mdl-list__item">
				<i class="fas fa-mobile"></i>
				<span class="mdl-list__item-primary-content">
					iOS systems (*): <?php echo mysqli_query($conn, "SELECT COUNT(*) FROM command WHERE `system` = '" . OS_IOS . "'")->fetch_column(); ?>
				</span>
			</li>
		</ul>
		Note:<br />
		(*) = Not implemented yet.
	</div>
</div>
