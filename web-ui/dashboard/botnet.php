<style>
	.demo-card-wide.mdl-card {
		width: 100%;
		min-height: inherit;
		padding: 15px;
	}

	.demo-card-wide>.mdl-card__menu {
		color: #fff;
	}

	td {
		border: 1px solid;
	}
</style>

<div class="mdl-grid">
	<div class="demo-card-wide mdl-card mdl-shadow--2dp">
		<h4>Botnet</h4>
		<table class="mdl-data-table mdl-js-data-table mdl-shadow--2dp">
			<thead>
				<tr>
					<th>Machine ID</th>
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
				$sql = "SELECT * FROM command ORDER BY last_signal DESC";
				$result = mysqli_query($conn, $sql);
				while ($row = $result->fetch_assoc()) {
					echo '<tr style="cursor: pointer;" onclick="redirectToMachine(\'' . $row['machineID'] . '\')" id="' . $row['machineID'] . '">
						<td>' . $row['machineID'] . '</td>
						<td><i class="fab fa-' . getFontAwesomeIconNameByOS($row['system']) . ' fa-2x"></i></td>
						<td>' . $row['programVersion'] . '</td>
						<td>Lat: ' . $row['latitude'] . '<br />Lon: ' . $row['longitude'] . '</td>
						<td>First: ' . $row['first_signal'] . '<br/>Last: ' . $row['last_signal'] . '</td>
						<td>';
					if ($row['note'] == "") echo "-";
					echo $row['note'] . '</td>
						<td>';
					if ($row['uninstalled']) echo '<i title="Uninstalled" class="fa fa-ban fa-2x"></i>';
					elseif ((time() - $row['ordersInterval']) < strtotime($row['last_signal'])) echo '<i title="Online" class="fa fa-circle-check fa-2x"></i>';
					else echo '<i title="Offline" class="fa fa-times-circle fa-2x"></i>';
					echo '</td>
					  </tr>';
				}
				?>
			</tbody>
		</table>
	</div>
</div>


<script>
	function redirectToMachine(machineID) {
		window.location.href = "/dashboard/?section=machine&machineID=" + machineID;
	}
</script>
