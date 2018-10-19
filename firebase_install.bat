echo this batch file is running to install firebase sdk
echo %1
echo %2

IF "%2%" == "Release" (

	cd %1
	
	echo "Reelase mode"
	echo "Copy path: %1%"
	
	npm install firebase --save-prod > fireabase_build.log
	npm install --save-prod socket.io-client > socketio_build.log
	
) ELSE (

	echo "is not release mode"
	echo "Copy path: %1%"
	
	cd %1
	
	echo "node_modules is exist? "
	
	IF exist "/node_modules" (
		echo "yes"
		goto eof;
	) ELSE (
		echo "no, install packages"
		npm install firebase --save-prod > fireabase_build.log
		npm install --save-prod socket.io-client > socketio_build.log
	)
)