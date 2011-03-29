// experiment.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"


int _tmain(int argc, _TCHAR* argv[])
{
    DWORD   dwProcessId = 8608;
    LPVOID  pvAddress   = (LPVOID  )0x003c5dd8;
    DWORD   dwSize = 10;

    BOOL bRet = 0;

    DWORD dwErr = GetLastError();
    SetLastError(0);

    HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, dwProcessId);
    {   
        char* buffer = new char[dwSize];
        for(DWORD i = 0; i < dwSize; i++){
            buffer[i] = 'a' + i;
        }

        SIZE_T szWritten = 0;
        bRet = WriteProcessMemory(hProcess, pvAddress, buffer, dwSize, &szWritten);
        if(FALSE == bRet){
            _tprintf(_T("%s"), _T("failed\n"));
        }
        dwErr = GetLastError();
        
        buffer[0] = 'b';
        bRet = ReadProcessMemory(hProcess, pvAddress, buffer, dwSize, &szWritten);

    }

    CloseHandle(hProcess);
    return 0;
}

