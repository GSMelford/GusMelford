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
        stage("Deploy") {
            steps {    
                echo "====== Deployment is requested... ======"
                sh "curl https://dev.gusmelford.com/job/infrastructure.gusmelfordbot/build?token=dfghjw4l489lsfdDFjh4"
                echo "====== Deploy launched ======"
            }
        }
    }
}