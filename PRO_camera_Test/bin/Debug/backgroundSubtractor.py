import numpy as np
import cv2
import matplotlib.pyplot as plt
from PIL import Image
#from PIL import ImageChop
from scipy.misc import imread
from scipy import ndimage as ndi
import time

#im1 = cv2.imread('images\\background.bmp')
#im2 = cv2.imread('images\\currentImage.bmp')

#im1 = cv2.imread('images\\background.bmp')
#im2 = cv2.imread('images\\currentImage.bmp')

# im1 = cv2.imread('images\\imageOfBackground.bmp')
# im2 = cv2.imread('images\\imageOfPadWithObject.bmp')

im1 = cv2.imread('TestCV191016_14-08-32.bmp')
im2 = cv2.imread('TestCV191016_14-07-30.bmp')
threshold = 50


current_frame_gray = cv2.cvtColor(im1, cv2.COLOR_BGR2RGB)
previous_frame_gray = cv2.cvtColor(im2, cv2.COLOR_BGR2RGB)

frame_diff = cv2.absdiff(im1, im2)

#cv2.imshow('frame diff ', frame_diff)
def histogram(im, size):
    h = np.zeros(size)
    for y in range(im.shape[0]):
        for x in range(im.shape[1]):
            h[im[y, x]] += 1
    return h

def histogramHue(im, size):
    h = np.zeros(size+1)
    import math
    rounded_value = 0
    for y in range(im.shape[0]):
        for x in range(im.shape[1]):
            if(im[y,x,0] + im[y,x,1] + im[y,x,2] != 0):
                hueValue, s, v = rgb2hsv(im[y,x,0], im[y,x,1], im[y,x,2])
                # rounded_value = math.ceil(hueValue)
                h[round(hueValue)] += 1
    return h

def rgb2hsv(r, g, b):
    r, g, b = r/255.0, g/255.0, b/255.0
    mx = max(r, g, b)
    mn = min(r, g, b)
    df = mx-mn
    if mx == mn:
        h = 0
    elif mx == r:
        h = (60 * ((g - b) / df))
        # h = (60 * ((g-b)/df) + 360) % 360
    elif mx == g:
        h = (60 * (2+(b - r) / df))
        # h = (60 * ((b-r)/df) + 120) % 360
    elif mx == b:
        h = (60 * (4+(r - g) / df))
        # h = (60 * ((r-g)/df) + 240) % 360
    if mx == 0:
        s = 0
    else:
        s = df/mx
    v = mx
    return h, s, v

def black_or_b(im1, im2):
    diff = cv2.subtract(im2, im1)
    # threshold(diff, diff, 30, 255, CV_THRESH_BINARY);
    #
    # cv2.Dilate(diff, diff, None, 18)  # to get object blobs
    # cv2.Erode(diff, diff, None, 10)

    plt.subplot(2,3,1)
    plt.imshow(im1)
    plt.axis("off")
    plt.subplot(2,3,2)
    plt.imshow(im2)
    plt.axis("off")
    plt.subplot(2,3,3)
    plt.imshow(diff)
    plt.axis("off")

    #img555 = cv2.medianBlur(diff, 5)
    img555 = cv2.cvtColor(diff, cv2.COLOR_RGB2GRAY)
    #cv2.imshow("img555",img555)

    plt.subplot(234)
    plt.imshow(img555, cmap='gray')
    plt.axis("off")

    #
    # h = histogram(diff)
    # plt.plot(h, '-r')
    # plt.figure()
    kernel = np.ones((5, 5), np.uint8)
    #ret,th3 = cv2.threshold(img555,threshold,255,cv2.THRESH_BINARY)
    threshold, th3 = cv2.threshold(img555, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)   #automaticky se vybere vhodny prah na zaklade hist, ktery se take zjisti sam
    print ("Auto threshold = {}".format(threshold))
    cv2.imshow('Auto threshold = {}'.format(threshold),th3)
    th3 = cv2.morphologyEx(th3, cv2.MORPH_OPEN, kernel)
    th3 = cv2.dilate(th3,kernel,iterations = 1)

    # #-----------------------
    # des = cv2.bitwise_not(th3)
    # qq, contour, hier = cv2.findContours(des, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    #
    # for cnt in contour:
    #     cv2.drawContours(des, [cnt], 0, 255, -1)
    #
    # th3 = cv2.bitwise_not(des)
    # # -----------------------

    #kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (13, 13))
    #res = cv2.morphologyEx(th3, cv2.MORPH_OPEN, kernel)

    # #======================
    # # Copy the thresholded image.
    # im_floodfill = th3.copy()
    #
    # # Mask used to flood filling.
    # # Notice the size needs to be 2 pixels than the image.
    # h, w = th3.shape[:2]
    # mask = np.zeros((h + 2, w + 2), np.uint8)
    #
    # # Floodfill from point (0, 0)
    # cv2.floodFill(im_floodfill, mask, (0, 0), 255);
    #
    # # Invert floodfilled image
    # im_floodfill_inv = cv2.bitwise_not(im_floodfill)
    #
    # # Combine the two images to get the foreground.
    # im_out = th3 | im_floodfill_inv
    # # ======================

    th33 = ndi.binary_fill_holes(th3)
    plt.subplot(2, 3, 5)
    plt.imshow(th33, 'gray')
    plt.axis("off")

    mask_inv = cv2.bitwise_not(th3)

    rows, cols, channels = im2.shape
    roi = im2[0:rows, 0:cols]

    # Now black-out the area of logo in ROI
    img1_bg = cv2.bitwise_and(roi, roi, mask =mask_inv)
    #cv2.imshow('img1_bg',img1_bg)
    #mask33 = cv2.bitwise_not(th33)
    # Take only region of logo from logo image.
    from skimage import img_as_ubyte
    cv_image = img_as_ubyte(th33)   #po vyplneni der se z nejakeho duvodu musi obrazek prevest zpet na format cv2...
    img2_fg = cv2.bitwise_and(im2, im2, mask=cv_image)
    img2_fg = cv2.cvtColor(img2_fg, cv2.COLOR_BGR2RGB)
    #cv2.imshow('img1_bg', img2_fg)
    #return diff


    im, contours, hierarchy = cv2.findContours(th3, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    #nalezeni nejvetsi oblasti
    largest_areas = sorted(contours, key=cv2.contourArea)
    cv2.drawContours(th3, contours, -1, (250, 250, 250), 2)
    # cv2.imshow('your_image.jpg', im_out)

    #########cv2.waitKey()
    #########cv2.destroyAllWindows()
    print("len(contours) = ",len(contours))
    if (len(contours)) != 0 :
        cnt = contours[0]
        cnt = largest_areas[-1]
        area = cv2.contourArea(cnt)
        # print(area)

        x, y, width, height = cv2.boundingRect(cnt)
        roi = img2_fg[y:y + height, x:x + width]
        #cv2.imshow('roi', roi)
        #cv2.imwrite("roi.png", roi)

        rect = cv2.minAreaRect(cnt)
        box = cv2.boxPoints(rect)
        box = np.int0(box)
        cv2.drawContours(im2, [box], 0, (0, 0, 255), 2)

        M = cv2.moments(cnt)
        cv2.drawContours(im2, [cnt], 0, (0, 255, 0), 3)
        #cv2.imshow('output', im2)

    plt.subplot(236)
    plt.imshow(cv2.cvtColor(roi, cv2.COLOR_BGR2RGB))
    plt.axis("off")
    plt.savefig('images\\saveFig.png')

    return roi

def foundObjectToGray(foundObject):
    img_gray = cv2.cvtColor(foundObject, cv2.COLOR_BGR2GRAY)
    hist_img_gray, b = np.histogram(img_gray, 255, (1, 255))
    maxXwMY = hist_img_gray.argmax()
    maxY = max(hist_img_gray)
    plt.figure('aaaaaaa',dpi=50)
    plt.subplot(121)
    plt.imshow(img_gray, cmap='gray')
    plt.subplot(122)
    plt.imshow(cv2.cvtColor(foundObject, cv2.COLOR_BGR2RGB))
    aaa = "Max = [{0}, {1}]".format(maxXwMY, maxY)
    print(aaa)
    plt.figure('hist_img_gray')
    #plt.plot(hist_img_gray)
    plt.plot(hist_img_gray, marker='.')
    plt.grid(True)
    plt.axvline(x=maxXwMY, ymin=0, ymax=maxXwMY, linewidth=1, color='r')
    plt.text(maxXwMY, maxY, aaa)
    plt.xlabel('GRAYSCALE VALUE')
    plt.ylabel('NUMBER')
    plt.title('Histogram of grayscale image')

    plt.savefig('images\\saveFig_Gray.png')
    # print(img_gray.mean())
    # print(hist_img_gray.mean())
    # print(hist_img_gray.max(axis=0))
    # print(np.argwhere(hist_img_gray == hist_img_gray.max()))
    print("hist_img_gray.argmax() = ",hist_img_gray.argmax())
    return maxXwMY


def foundObject_hue(foundObject):
    img_hsv = cv2.cvtColor(foundObject, cv2.COLOR_BGR2HSV)
    # cv2.imshow('images\\aaaaaaaaaaaaaaaa', foundObject)
    #hist_hue_img, b = np.histogram(img_hsv[:, :, 0], 360, (0, 256))
    #hist_hue_img = histogram(img_hsv[:, :, 0], 360)
    #hist_hue_img, s, v = rgb2hsv(foundObject[:,0,0],0,0)
    hist_hue_img = histogramHue(cv2.cvtColor(foundObject, cv2.COLOR_BGR2RGB), 360)

    # cv2.imshow('hsvHrnek', img_hsv)
    maxXwhereMaxY = hist_hue_img.argmax()
    maxY = max(hist_hue_img)
    aaa = "Max = [{0}, {1}]".format(maxXwhereMaxY, int(maxY))
    print("Hue ",aaa)

    imgBack = plt.imread("HueScale.png")

    plt.figure('hist_hue_img', figsize=(6, 5), dpi=100)
    plt.axis('auto')
    #plt.autoscale_view(True, True, True)
    plt.xlim(0,360)
    plt.ylim(0,maxY+200)
    plt.imshow(imgBack, zorder=0, extent=[0, 360, 0, maxY+200],  aspect='auto')
    plt.plot(hist_hue_img, marker='.', zorder=1, color='black', linewidth=2, markeredgewidth=3)
    plt.grid(True)
    plt.text(maxXwhereMaxY, maxY, aaa)
    plt.title('Histogram for hue values')
    plt.xlabel('HUE VALUE')
    plt.ylabel('NUMBER')

    plt.savefig('images\\saveFig_Hue.png')
    # print(img_hsv[:, :, 0].mean())
    # print(hist_hue_img.mean())
    # print(hist_hue_img.max(axis=0))
    # print(np.argwhere(hist_hue_img == hist_hue_img.max()))
    print("hist_hue_img.argmax() = ",hist_hue_img.argmax())
    return maxXwhereMaxY

#import mahotas
foundObject = black_or_b(current_frame_gray, previous_frame_gray).astype(np.uint8)

#qqq = cv2.imread('HueScale.png')
gray = foundObjectToGray(foundObject)
hue = foundObject_hue(foundObject)
bbb = "{0} = Hue".format(round(hue))
ccc = "{0} = Gray".format(gray)
print (bbb)
print (ccc)
# Write some Text
cv2.putText(foundObject,bbb,(0,10), cv2.FONT_HERSHEY_SIMPLEX, 0.3,(255,255,255),1)
cv2.putText(foundObject,ccc,(0,20), cv2.FONT_HERSHEY_SIMPLEX, 0.3,(255,255,255),1)


ts = int(time.time())
immmm = Image.fromarray(cv2.cvtColor(foundObject, cv2.COLOR_BGR2RGB))
immmm.save("images\\objectFound{}.bmp".format(ts))
#cv2.imshow('images\\aaaaaaaaaaaaaaaa', foundObject)

# import colorsys
# r, g, b = 0, 0, 255
# h, l, s = colorsys.rgb_to_hls(r, g, b)
# r, g, b = colorsys.hls_to_rgb(h, l, s)
# h, l, s = rgb2hsv(r, g, b)
# print (r, g, b)
# print (h, l, s)


# print(img_gray[:,:,0].mean())
# print(img_gray.mean())
# print(img_gray.max(axis=0))
# print(np.argwhere(img_gray == img_gray.max()))
# print(img_gray.argmax())



plt.show()