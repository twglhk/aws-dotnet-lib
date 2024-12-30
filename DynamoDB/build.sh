#!/bin/bash

# .NET 프로젝트 디렉토리
PROJECT_DIR="."

# .NET 프로젝트 파일
PROJECT_FILE="WardGames.Web.Dotnet.AWS.DynamoDB.csproj"

# .NET 패키지 이름
PACKAGE_NAME="WardGames.Web.Dotnet.AWS.DynamoDB"

# NuGet 피드 이름
FEED_NAME="WardGames"

# NuGet API 키
API_KEY="az"

# 입력 파라미터를 받아옴 (Debug 또는 Release)
BUILD_TYPE=$1

if [ "$BUILD_TYPE" != "Debug" ] && [ "$BUILD_TYPE" != "Release" ]; then
    echo "Invalid input. Please provide either 'Debug' or 'Release' as an argument."
    exit 1
fi

# 백업 폴더
BACKUP_DIR="./bin/$BUILD_TYPE/backup"

# dotnet build 실행
dotnet build $PROJECT_DIR --configuration $BUILD_TYPE

if [ $? -eq 0 ]; then
    echo "Build successful. Running dotnet pack..."
    dotnet pack $PROJECT_DIR --configuration $BUILD_TYPE
    if [ $? -eq 0 ]; then
        PACKAGE_FILE="$PROJECT_DIR/bin/$BUILD_TYPE/$PACKAGE_NAME.*.nupkg"
        echo "Pack successful. Pushing to NuGet feed..."
        dotnet nuget push $PACKAGE_FILE --source $FEED_NAME --api-key $API_KEY
        if [ $? -eq 0 ]; then
            echo "Push successful. Moving the package to the backup folder..."
            mv $PACKAGE_FILE $BACKUP_DIR
            if [ $? -eq 0 ]; then
                echo "Package successfully moved to the backup folder."
            else
                echo "Failed to move the package."
            fi
        else
            echo "Push failed. Removing the package..."
            rm $PACKAGE_FILE
            if [ $? -eq 0 ]; then
                echo "Package removed successfully."
            else
                echo "Failed to remove the package."
            fi
        fi
    else
        echo "Pack failed."
    fi
else
    echo "Build failed."
fi
