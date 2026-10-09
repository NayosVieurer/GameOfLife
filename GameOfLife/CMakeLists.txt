cmake_minimum_required(VERSION 3.15)
project(GameOfLife CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Récupère AUTOMATIQUEMENT tous les .cpp, .h et .hpp, même dans les sous-dossiers
file(GLOB_RECURSE SOURCES CONFIGURE_DEPENDS "*.cpp" "*.h" "*.hpp")

add_executable(GameOfLife ${SOURCES})

# Inclut automatiquement tous les dossiers trouvés pour les includes
target_include_directories(GameOfLife PRIVATE .)