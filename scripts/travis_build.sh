#!/bin/sh -ue

return_unity_license () {
  # return the Unity license as we can only have two simultaneous activations of each license
  /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense -logFile unity.log
  # usage suggests that sometimes this can take longer than the command so wait for the above command to take effect
  sleep 10
}

handle_failure () {
  exit_code=${1:-}
  if [[ $exit_code -ne 0 ]]; then
    return_unity_license
    exit $exit_code
  fi
}

brew install mono
brew tap caskroom/cask
brew cask install android-sdk

yes | sdkmanager "platforms;android-27"
yes | sdkmanager --licenses

curl -o Unity.pkg https://download.unity3d.com/download_unity/21ae32b5a9cb/MacEditorInstaller/Unity-2017.4.3f1.pkg
sudo installer -dumplog -package Unity.pkg -target /
mv /Applications/Unity /Applications/Unity2017

curl -o Unity.pkg https://netstorage.unity3d.com/unity/1431a7d2ced7/MacEditorInstaller/Unity-2018.2.3f1.pkg
sudo installer -dumplog -package Unity.pkg -target /
mv /Applications/Unity /Applications/Unity2018

ln -s /Applications/Unity2017 /Applications/Unity

# activate Unity
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -serial $UNITY_SERIAL -username $UNITY_USERNAME -password $UNITY_PASSWORD -logFile unity.log
# usage suggests that sometimes this can take longer than the command so wait for the above command to take effect
sleep 10

# turn off errexit here as we need to return the unity license even if these commands fail
set +e

# build the plugin
UNITY_DIR=/Applications/Unity2017 bundle exec rake plugin:export && handle_failure

# copy it to the directory that is being synchronised with S3
cp Bugsnag.unitypackage ~/$TRAVIS_BUILD_NUMBER && handle_failure

mkdir -p ~/$TRAVIS_BUILD_NUMBER/2017 && handle_failure
UNITY_DIR=/Applications/Unity2017 bundle exec rake travis:build_applications && handle_failure
UNITY_DIR=/Applications/Unity2017 bundle exec rake travis:build_applications && handle_failure
cp -r features/fixtures/mazerunner.app ~/$TRAVIS_BUILD_NUMBER/2017/mazerunner.app && handle_failure
cp features/fixtures/mazerunner.apk ~/$TRAVIS_BUILD_NUMBER/2017/mazerunner.apk && handle_failure

mkdir -p ~/$TRAVIS_BUILD_NUMBER/2018 && handle_failure
UNITY_DIR=/Applications/Unity2018 bundle exec rake travis:build_applications && handle_failure
UNITY_DIR=/Applications/Unity2018 bundle exec rake travis:build_applications && handle_failure
cp -r features/fixtures/mazerunner.app ~/$TRAVIS_BUILD_NUMBER/2018/mazerunner.app && handle_failure
cp features/fixtures/mazerunner.apk ~/$TRAVIS_BUILD_NUMBER/2018/mazerunner.apk && handle_failure

set -e

return_unity_license
