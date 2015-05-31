#!/usr/bin/env python

import argparse
import os
import re
import time
import filecmp
import shutil
import glob

version = 'v2.1.0'
ignored = {
	'Thumbs.db'
}
fileNumberPattern = re.compile('([0-9]+)')
argParser = argparse.ArgumentParser(prog='savepics',
	description='Sorts picture files from an input directory into an output directory, using a folder structure with the pattern YYYY/YYYY.MM SUFFIX.'
)
argParser.add_argument('--version', action='version', version='%(prog)s {}'.format(version))
argParser.add_argument('-n', '--dry-run', action='store_true',
	help='prints out what would be written without actually writing'
)
argParser.add_argument('-s', '--suffix', action='store', default='Pictures', const=None, nargs='?',
	help='suffix to insert after folder name, defaults to %(default)s, can be overriden to nothing'
)
argParser.add_argument('-v', '--verbose', action='count',
	help='enable extra output'
)
argParser.add_argument('-m', '--month', action='store', type=int, choices=range(1, 13), metavar='MONTH',
	help='override the detected month of files for sorting purposes'
)
argParser.add_argument('-y', '--year', action='store', type=int,
	help='override the detected year of files for sorting purposes'
)
argParser.add_argument('input', action='store',
	help='directory to read and sort'
)
argParser.add_argument('output', action='store',
	help='directory to write sorted pictures to'
)

def getFileNumber(filename):
	"""finds the number in a picture filename"""
	result = fileNumberPattern.search(filename)  
	return result.group(0) if result else '0'
	
def getFileYearMonth(filename, args):
	"""gets the year and month a picture was modified (taken)"""
	mtime = time.localtime(os.path.getmtime(filename))
	return args.year or mtime.tm_year, args.month or mtime.tm_mon

def walkFiles(inDir):
	"""will iterate over non-ignored files"""
	for path, dirnames, filenames in os.walk(inDir):
		for filename in sorted(filenames):
			if filename in ignored:
				continue
			yield os.path.join(path, filename)
			
def getFileOutPath(filename, outDir, args):
	year, month = getFileYearMonth(filename, args)
	formatStr = '{}.{:02}'
	if args.suffix:
		formatStr += ' {}'
	return os.path.join(outDir, str(year), formatStr.format(year, month, args.suffix), os.path.basename(filename))
	
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
		outpath = getFileOutPath(filename, outDir, args)
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
		dirname = os.path.dirname(output)
		if not os.path.exists(dirname):
			os.makedirs(os.path.dirname(output))
		shutil.copy2(input, output)
		
	print('{} pictures scanned.'.format(fileCount))
	print('{} pictures copied.'.format(len(copies)))
	print('{} duplicates found.'.format(len(duplicates)))
	if args.verbose:
		for duplicate in sorted(duplicates):
			print('\t{}'.format(duplicate))
	print('{} renames.'.format(len(renames)))
	if args.verbose:
		for rename in sorted(renames):
			print('\t{}'.format(rename))

if __name__ == '__main__':
	main()