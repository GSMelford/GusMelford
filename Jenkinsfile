#!groovy

pipeline {
    agent any
    options {
        timestamps()
        disableConcurrentBuilds()
    }

    stages {
        stage("Build GusMelfordBot") {
            steps {
                echo "====== Building image... ======"
                sh "sudo docker build -t gsmelford/gusmelfordbot:latest ."
                echo "====== Build completed ======"
            }
        }
        stage("Build GusMelford.ContentCollector") {
            steps {    
                echo "====== Building image... ======"
                sh "sudo docker build -f contentcollector.Dockerfile -t gsmelford/gusmelfordbot.contentcollector:latest ."
                echo "====== Build completed ======"
            }
        }
    }
}
