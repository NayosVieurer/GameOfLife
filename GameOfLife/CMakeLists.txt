cmake_minimum_required(VERSION 3.15)
project(GameOfLife CXX)

# Configuration du standard C++
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Activation indispensable des threads pour MinGW (résout l'erreur std::thread)
add_compile_options(-mthreads)
add_link_options(-mthreads)

# Récupération automatique de tous les fichiers sources (.cpp) et en-têtes (.h, .hpp)
file(GLOB_RECURSE SOURCES CONFIGURE_DEPENDS "*.cpp" "*.h" "*.hpp")

# Création de l'exécutable Windows
add_executable(GameOfLife ${SOURCES})

# Inclusion automatique des dossiers pour les en-têtes
target_include_directories(GameOfLife PRIVATE .)