#include <iostream>

#include "bitboard.h"
#include "evaluate.h"
#include "position.h"
#include "search.h"
#include "thread.h"
#include "tt.h"
#include "uci.h"
#include "tbprobe.h"

#include "unity.h"

namespace PSQT {
    void init();
}

typedef void (*UnityOutFunc)( const char * );
UnityOutFunc unityOutFunc;

extern "C"
{
    void tl_setUnityOutFuncPtr( UnityOutFunc fp )
    {
      unityOutFunc = fp;
    }

    void tl_echo( const char *arg );

    void tl_init();

    void tl_cmd( const char *arg );

    void tl_shutdown();
}

void unityOut( std::string output ) {
	unityOutFunc(output.c_str());
}

void tl_echo( const char* arg ) {
  unityOut( "Echo: " + std::string(arg) );
}

void tl_init() {
  unityOut("AI Version 92018");

  UCI::init(Options);
  PSQT::init();
  Bitboards::init();
  Position::init();
  Bitbases::init();
  Search::init();
  Pawns::init();
  Tablebases::init(Options["SyzygyPath"]);
  TT.resize(Options["Hash"]);
  Threads.set(Options["Threads"]);
  Search::clear(); // After threads are up
}

void tl_cmd( const char *arg ) {
	UCI::unitycmd( std::string(arg) );
}

void tl_shutdown() {
	Threads.set(0);
}
