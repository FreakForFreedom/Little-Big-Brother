function donothing(server)
end

function spawnasteroids(server)
	server:InstantiateAsteroid();
end

function asteroidminingstarted(server)
	server:OnPlayerMiningStarted();
end

function playerdestroysasteroid(server)
	server:OnPlayerDestroysAsteroid();
end

function playerattacks(server)
	server:OnPlayerAttacks();
end

function attackallplayers(server)
	server:AttackAllPlayers();
end

function fleefromplayers(server)
	server:FleeFromPlayers();
end

function spawnrebels(server)
	server:SpawnRebels();
end

function gameover(server)
	server:GameOver();
end

function gamewon(server)
	server:GameWon();
end

function stopattackingyourfriends(server)
	server:OnAttackFriends();
end