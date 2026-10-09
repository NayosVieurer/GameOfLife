cmake_minimum_required(VERSION 3.15)
project(GameOfLife CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Forcer explicitement les drapeaux de threads MinGW
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -mthreads")
set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -mthreads")

file(GLOB_RECURSE SOURCES CONFIGURE_DEPENDS "*.cpp" "*.h" "*.hpp")
add_executable(GameOfLife ${SOURCES})
target_include_directories(GameOfLife PRIVATE .)