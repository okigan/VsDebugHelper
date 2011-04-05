// TestProjectCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

typedef unsigned char lum8;

struct rgba {
    unsigned char r, g, b, a;
};


int _tmain(int argc, _TCHAR* argv[])
{
    int width = 256;
    int height = width;

    std::auto_ptr<lum8> grayPixels(new lum8[width*height]);
        
    {
        lum8* row = grayPixels.get();

        for(int r = 0, rc = height; r < rc; ++r){
            for(int c = 0, cc = width; c < cc; ++c, ++row){
                (*row) = c;
            }
        }
    }

    std::auto_ptr<rgba> colorPixels(new rgba[width*height]);

    {
        rgba* row = colorPixels.get();

        for(int r = 0, rc = height; r < rc; ++r){
            for(int c = 0, cc = width; c < cc; ++c, ++row){
                row->r = c;
                row->g = r;
                row->b = 0;
                row->a = 255;
            }
        }
    }

    //VSDebugHelper.writemem c:\temp\gray.png grayPixels.get() width*height*sizeof(lum8)
    //VSDebugHelper.writemem c:\temp\color.png colorPixels.get() width*height*sizeof(rgba)

    return 0;
}

