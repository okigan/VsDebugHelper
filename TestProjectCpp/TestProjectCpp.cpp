// TestProjectCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

typedef unsigned char lum8;

int _tmain(int argc, _TCHAR* argv[])
{
    int width = 256;
    int heigt = 256;

    lum8* pixels = new lum8[width*heigt];
    lum8* row = pixels;

    for(int r = 0, rc = heigt; r < rc; ++r, row += width){
        for(int c = 0, cc = width; c < cc; ++c){
            row[c] = c;
        }
    }

    return 0;
}

//VSDebugHelper.writemem c:\temp\data.raw pixels width*heigt