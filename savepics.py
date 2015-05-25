#!/usr/bin/env python

import argparse
import os
import re
import time
import filecmp
import shutil
import glob

version = 'v2.0.0'
ignored = {
	'Thumbs.db'
}
fileNumberPattern = re.compile('([0-9]+)')
argParser = argparse.ArgumentParser(prog='savepics',
	description='Sorts picture files from an input directory into an output directory, using a folder structure with the pattern YYYY/YYYY.MM.'
)
argParser.add_argument('-v', '--version', action='version', version='%(prog)s {}'.format(version))
argParser.add_argument('-n', '--dry-run', action='store_true',
	help='Prints out what would be written without actually writing.'
)
argParser.add_argument('-s', '--suffix', action='store', default='Pictures',
	help='Suffix to insert after folder name, defaults to %(default)s.'
)
argParser.add_argument('input', action='store',
	help='Directory to read and sort.'
)
argParser.add_argument('output', action='store',
	help='Directory to write sorted pictures to.'
)

def getFileNumber(filename):
	"""finds the number in a picture filename"""
	result = fileNumberPattern.search(filename)  
	return result.group(0) if result else '0'
	
def getFileYearMonth(filename):
	"""gets the year and month a picture was modified (taken)"""
	mtime = time.localtime(os.path.getmtime(filename))
	return mtime.tm_year, mtime.tm_mon

def walkFiles(inDir):
	"""will iterate over non-ignored files"""
	for path, dirnames, filenames in os.walk(inDir):
		for filename in sorted(filenames):
			if filename in ignored:
				continue
			yield os.path.join(path, filename)
			
def getFileOutPath(filename, outDir, suffix):
	year, month = getFileYearMonth(filename)
	return os.path.join(outDir, str(year), '{}.{:02} {}'.format(year, month, suffix), os.path.basename(filename))
	
def splitFilename(filename):
	"""splits filename into dirname, basename, ext"""	
	dirname, basename = os.path.split(filename)
	basename, ext = os.path.splitext(basename)
	return dirname, basename, ext
	
def getNextFilename(filename):
	"""when a filename already exists for another picture this function can increment the number"""
	dirname, basename, ext = splitFilename(filename)
	number = getFileNumber(basename)
	width = len(number)
	formatStr = '{:0' + str(width) + '}'
	basename = basename.replace(
		number,
		formatStr.format(int(number) + 1)
	)
	return os.path.join(dirname, basename + ext)
	
def getOrInsert(dictObj, key, default):
	value = dictObj.get(key, None)
	if value is None:
		dictObj[key] = default
		value = default
		
	return value

cachedBasenameMap = {}
def isFilenameUsed(filename):
	"""this function will check if a basename is in use for a file means that IMG_1111.MOV will be "in use" if IMG_1111.JPG exists"""
	dirname, basename, ext = splitFilename(filename)
	cachedBasenames = getOrInsert(cachedBasenameMap, dirname, set())
	if basename in cachedBasenames:
		return True
		
	globPattern = filename[:-(len(ext) - 1)] + '*'
	for match in glob.iglob(globPattern):
		cachedBasenames.add(basename)
		return True
	
	return False
	
def useFilename(filename):
	"""updates the above cached dict with a new basename usage"""
	dirname, basename, ext = splitFilename(filename)
	cachedBasenameMap[dirname].add(basename)

def main():
	args = argParser.parse_args()
	
	inDir = os.path.abspath(args.input)
	outDir = os.path.abspath(args.output)
	copies = []
	fileCount = 0
	duplicates = set()
	renames = set()
	for filename in walkFiles(inDir):
		fileCount += 1
		outpath = getFileOutPath(filename, outDir, args.suffix)
		# see if this name is already in use
		while isFilenameUsed(outpath):
			# see if this file has already been copied
			if os.path.exists(outpath) and filecmp.cmp(filename, outpath, False):
				duplicates.add(filename)
				break
			# find a new name
			else:
				renames.add(filename)
				outpath = getNextFilename(outpath)
		
		if filename in duplicates:
			if filename in renames:
				renames.remove(filename)
			continue
			
		useFilename(outpath)
		copies.append((filename, outpath))
		
	for input, output in copies:
		print('Copying {} to {}.'.format(input, output))
		if args.dry_run:
			continue
		shutil.copy2(input, output)
		
	print('{} pictures scanned.'.format(fileCount))
	print('{} pictures copied.'.format(len(copies)))
	print('{} duplicates found.'.format(len(duplicates)))
	print('{} renames.'.format(len(renames)))

if __name__ == '__main__':
	main()