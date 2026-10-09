pipeline {
    agent any

    stages {
        stage('Récupération du code') {
            steps {
                // Utilise le plugin Git de Jenkins
                checkout scm
            }
        }

	stage('Build C# (GameOfLifeClient)') {
            steps {
                sh 'dotnet restore GameOfLifeClient/GameOfLifeClient.csproj'
                sh 'dotnet build GameOfLifeClient/GameOfLifeClient.csproj --configuration Release'
            }
        }

        stage('Build C++ (GameOfLife)') {
            steps {
                dir('GameOfLife') {
		sh '''
           	    cmake -B build -S . \
    	            -DCMAKE_SYSTEM_NAME=Windows \
      	            -DCMAKE_CXX_COMPILER=x86_64-w64-mingw32-g++ \
                    -DCMAKE_BUILD_TYPE=Release
                    cmake --build build --config Release
                '''
                }
            }
        }

        stage('Tests') {
            steps {
                // Exécution des tests unitaires
                sh 'dotnet test GameOfLife.sln'
                // sh 'build/tests/unit_tests' // Pour le C++
            }
        }
    }

    post {
        always {
            echo 'Nettoyage ou archivage des artefacts si nécessaire.'
        }
        failure {
            echo 'Le build a échoué !'
        }
    }
}
