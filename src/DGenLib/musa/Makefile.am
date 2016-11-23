# DGen/SDL v1.26+
# Automakefile for Musashi

CC_FOR_BUILD=@CC_FOR_BUILD@
CFLAGS_FOR_BUILD=@CFLAGS_FOR_BUILD@
OBJEXT_FOR_BUILD=@OBJEXT_FOR_BUILD@
EXEEXT_FOR_BUILD=@EXEEXT_FOR_BUILD@

M68KMAKE = m68kmake$(EXEEXT_FOR_BUILD)
DIR = $(top_srcdir)/musa

$(M68KMAKE): $(DIR)/m68kmake.c $(DIR)/m68k_in.c
	$(CC_FOR_BUILD) $(CFLAGS_FOR_BUILD) -o $@ $(DIR)/m68kmake.c

BUILT_SOURCES = m68kops.c m68kops.h

$(BUILT_SOURCES): $(M68KMAKE)
	./$(M68KMAKE) . $(DIR)/m68k_in.c

noinst_LIBRARIES = libmusa68.a
libmusa68_a_SOURCES =	\
	m68kcpu.c	\
	m68kcpu.h	\
	m68k.h		\
	m68kconf.h	\
	m68kdasm.c	\
	$(BUILT_SOURCES)
libmusa68_a_DEPENDENCIES = $(M68KMAKE)
EXTRA_libmusa68_a_SOURCES = m68k_in.c m68kmake.c

EXTRA_DIST = history.txt readme.txt

DISTCLEANFILES = $(BUILT_SOURCES) $(M68KMAKE)

.NOTPARALLEL:
