# DGen/SDL v1.26+
# Automakefile for StarScream

include $(top_srcdir)/Makefile.asm.am

CC_FOR_BUILD=@CC_FOR_BUILD@
CFLAGS_FOR_BUILD=@CFLAGS_FOR_BUILD@
OBJEXT_FOR_BUILD=@OBJEXT_FOR_BUILD@
EXEEXT_FOR_BUILD=@EXEEXT_FOR_BUILD@

STAR = star$(EXEEXT_FOR_BUILD)
DIR = $(top_srcdir)/star

$(STAR): $(DIR)/star.c
	$(CC_FOR_BUILD) $(CFLAGS_FOR_BUILD) -o $@ $<

BUILT_SOURCES = starcpu.asm

$(BUILT_SOURCES): $(STAR)
	./$(STAR) -hog $(BUILT_SOURCES)

noinst_LIBRARIES = libstarcpu.a
libstarcpu_a_SOURCES = $(BUILT_SOURCES)
libstarcpu_a_DEPENDENCIES = $(STAR)
EXTRA_libstarcpu_a_SOURCES = star.c

# Safety net, so all of StarScream is distributed (as per the StarScream
# license)
EXTRA_DIST = cpudebug.c cpudebug.h star.c starcpu.h stardoc.txt

DISTCLEANFILES = $(BUILT_SOURCES) $(STAR)

.NOTPARALLEL:
